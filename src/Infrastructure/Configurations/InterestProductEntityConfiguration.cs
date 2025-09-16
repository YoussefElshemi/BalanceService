using Infrastructure.Constants;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class InterestProductEntityConfiguration : IEntityTypeConfiguration<InterestProductEntity>
{
    public void Configure(EntityTypeBuilder<InterestProductEntity> builder)
    {
        builder.HasKey(x => x.InterestProductId);
        builder.Property(x => x.RowVersion).IsRowVersion();

        builder
            .HasOne(x => x.InterestPayoutFrequencyEntity)
            .WithMany(x => x.InterestProductEntities)
            .HasForeignKey(x => x.InterestPayoutFrequencyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.ToTable(TableNames.InterestProducts);
    }
}