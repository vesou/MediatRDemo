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
            modelBuilder.Entity<Bid>()
                .Property(x => x.Amount)
                .HasColumnType("money");

            modelBuilder.Entity<Vehicle>()
                .HasKey(x => x.Id);

            base.OnModelCreating(modelBuilder);
        }
    }
}