using Infrastructure.Constants;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class HoldHistoryEntityConfiguration : IEntityTypeConfiguration<HoldHistoryEntity>
{
    public void Configure(EntityTypeBuilder<HoldHistoryEntity> builder)
    {
        builder.HasKey(x => x.HoldHistoryId);

        builder.HasIndex(x => x.HoldId);

        builder
            .HasOne(x => x.HistoryTypeEntity)
            .WithMany(x => x.HoldHistoryEntities)
            .HasForeignKey(x => x.HistoryTypeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(x => x.AccountEntity)
            .WithMany()
            .HasForeignKey(x => x.AccountId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(h => h.SettledTransactionEntity)
            .WithOne()
            .HasForeignKey<HoldHistoryEntity>(h => h.SettledTransactionId)
            .OnDelete(DeleteBehavior.SetNull);

        builder
            .HasOne(x => x.HoldTypeEntity)
            .WithMany()
            .HasForeignKey(x => x.HoldTypeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(x => x.HoldSourceEntity)
            .WithMany()
            .HasForeignKey(x => x.HoldSourceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(x => x.HoldStatusEntity)
            .WithMany()
            .HasForeignKey(x => x.HoldStatusId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.ToTable(TableNames.HoldHistory);
    }
}