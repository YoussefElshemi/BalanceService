using Infrastructure.Constants;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class HoldEntityConfiguration : IEntityTypeConfiguration<HoldEntity>
{
    public void Configure(EntityTypeBuilder<HoldEntity> builder)
    {
        builder.HasKey(x => x.HoldId);
        builder.Property(x => x.RowVersion).IsRowVersion();

        builder.HasIndex(x => x.IdempotencyKey).IsUnique();

        builder
            .HasOne(x => x.AccountEntity)
            .WithMany(x => x.HoldEntities)
            .HasForeignKey(x => x.AccountId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(h => h.SettledTransactionEntity)
            .WithOne()
            .HasForeignKey<HoldEntity>(h => h.SettledTransactionId)
            .OnDelete(DeleteBehavior.SetNull);

        builder
            .HasOne(x => x.HoldTypeEntity)
            .WithMany(x => x.HoldEntities)
            .HasForeignKey(x => x.HoldTypeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(x => x.HoldSourceEntity)
            .WithMany(x => x.HoldEntities)
            .HasForeignKey(x => x.HoldSourceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(x => x.HoldStatusEntity)
            .WithMany(x => x.HoldEntities)
            .HasForeignKey(x => x.HoldStatusId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.ToTable(TableNames.Holds);
    }
}