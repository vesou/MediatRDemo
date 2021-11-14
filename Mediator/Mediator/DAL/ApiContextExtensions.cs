using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Mediator.DAL
{
    public static class ApiContextExtensions
    {
        public static Task EnableIdentityInsert<T>(this ApiContext context) => SetIdentityInsert<T>(context, enable: true);
        public static Task DisableIdentityInsert<T>(this ApiContext context) => SetIdentityInsert<T>(context, enable: false);

        private static Task SetIdentityInsert<T>(ApiContext context, bool enable)
        {
            var entityType = context.Model.FindEntityType(typeof(T));
            var value = enable ? "ON" : "OFF";
            return context.Database.ExecuteSqlRawAsync(
                $"SET IDENTITY_INSERT dbo.{entityType.GetTableName()} {value}");
        }

        public static void SaveChangesWithIdentityInsert<T>(this ApiContext context)
        {
            using var transaction = context.Database.BeginTransaction();
            context.EnableIdentityInsert<T>();
            context.SaveChanges();
            context.DisableIdentityInsert<T>();
            transaction.Commit();
        }

    }
}