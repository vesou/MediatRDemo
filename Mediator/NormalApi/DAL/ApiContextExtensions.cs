using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace NormalApi.DAL
{
    public static class ApiContextExtensions
    {
        public static async Task SaveChangesWithIdentityInsert<T>(this ApiContext context)
        {
            await using var transaction = await context.Database.BeginTransactionAsync();
            await context.EnableIdentityInsert<T>();
            await context.SaveChangesAsync();
            await context.DisableIdentityInsert<T>();
            await transaction.CommitAsync();
        }

        private static Task DisableIdentityInsert<T>(this ApiContext context)
        {
            return SetIdentityInsert<T>(context, false);
        }

        private static Task EnableIdentityInsert<T>(this ApiContext context)
        {
            return SetIdentityInsert<T>(context, true);
        }

        private static Task SetIdentityInsert<T>(ApiContext context, bool enable)
        {
            var entityType = context.Model.FindEntityType(typeof(T));
            var value = enable ? "ON" : "OFF";
            return context.Database.ExecuteSqlRawAsync(
                $"SET IDENTITY_INSERT dbo.{entityType.GetTableName()} {value}");
        }
    }
}