using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NormalApi.DAL.Models;

namespace NormalApi.DAL.EntityConfigurations
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