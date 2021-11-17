using System.Linq;
using System.Threading.Tasks;
using Mediator.DAL.Models;

namespace Mediator.DAL
{
    public static class DbInitializer
    {
        public static async Task Initialize(ApiContext context)
        {
            await context.Database.EnsureCreatedAsync();

            // Look for any students.
            if (context.Vehicles.Any())
            {
                return; // DB has been seeded
            }

            var vehicle = new Vehicle { Id = 1, Make = "Audi", Model = "A6" };
            await context.Vehicles.AddAsync(vehicle);
            await context.SaveChangesWithIdentityInsert<Vehicle>();


            var bid = new Bid { Id = 1, User = "Simon", Amount = 1000, VehicleId = vehicle.Id };
            await context.Bids.AddAsync(bid);
            await context.SaveChangesWithIdentityInsert<Bid>();
        }
    }
}