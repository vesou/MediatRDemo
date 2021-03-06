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

            if (context.Vehicles.Any())
            {
                return; // DB has been seeded
            }

            var vehicles = new[]
            {
                new Vehicle { VehicleId = 1, Make = "Audi", Model = "A6", OnSale = true },
                new Vehicle { VehicleId = 2, Make = "Skoda", Model = "Fabia", OnSale = true },
                new Vehicle { VehicleId = 3, Make = "Tesla", Model = "Model 3", OnSale = false }
            };
            await context.Vehicles.AddRangeAsync(vehicles);
            await context.SaveChangesWithIdentityInsert<Vehicle>();


            var bid = new Bid { Id = 1, Amount = 10000, VehicleId = 2 };
            await context.Bids.AddAsync(bid);
            await context.SaveChangesWithIdentityInsert<Bid>();
        }
    }
}