using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mediator.DAL.Models;
using Microsoft.EntityFrameworkCore;

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

            var vehicle = new Vehicle { Make = "Audi", Model = "A5" };
            await context.Vehicles.AddAsync(vehicle);
            await context.SaveChangesAsync();
            
            var bid = new Bid { User = "Simon", Amount = 1000, VehicleId = vehicle.Id};
            await context.Bids.AddAsync(bid);
            await context.SaveChangesAsync();
        }
    }
}