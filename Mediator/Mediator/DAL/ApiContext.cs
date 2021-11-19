using Mediator.DAL.EntityConfigurations;
using Mediator.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace Mediator.DAL
{
    public class ApiContext : DbContext
    {
        public ApiContext(DbContextOptions<ApiContext> options)
            : base(options)
        {
        }

        public DbSet<Bid> Bids { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new BidsConfiguration());
            modelBuilder.ApplyConfiguration(new VehiclesConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}