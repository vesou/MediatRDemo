using Mediator.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mediator.DAL.EntityConfigurations
{
    public class BidsConfiguration : IEntityTypeConfiguration<Bid>
    {
        public void Configure(EntityTypeBuilder<Bid> builder)
        {
            builder
                .Property(x => x.Amount)
                .HasColumnType("money");
        }
    }
}