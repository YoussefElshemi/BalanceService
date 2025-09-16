using Infrastructure.Constants;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class InterestAccrualEntityConfiguration : IEntityTypeConfiguration<InterestAccrualEntity>
{
    public void Configure(EntityTypeBuilder<InterestAccrualEntity> builder)
    {
        builder.HasKey(x => x.InterestAccrualId);
        builder.Property(x => x.RowVersion).IsRowVersion();

        builder
            .HasOne(x => x.AccountEntity)
            .WithMany(x => x.InterestAccrualEntities)
            .HasForeignKey(x => x.AccountId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(x => x.InterestProductEntity)
            .WithMany(x => x.InterestAccrualEntities)
            .HasForeignKey(x => x.InterestProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.ToTable(TableNames.InterestAccruals);
    }
}