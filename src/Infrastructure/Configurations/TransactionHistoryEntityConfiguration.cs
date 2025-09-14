using Core.Enums;
using Infrastructure.Constants;
using Infrastructure.Entities.History;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class TransactionHistoryEntityConfiguration : IEntityTypeConfiguration<TransactionHistoryEntity>
{
    public void Configure(EntityTypeBuilder<TransactionHistoryEntity> builder)
    {
        builder.HasKey(x => x.TransactionHistoryId);
        builder.Property(x => x.RowVersion).IsRowVersion();
        builder.Property(x => x.ProcessingStatusId).HasDefaultValue((int)ProcessingStatus.NotProcessed);

        builder.HasIndex(x => x.TransactionId);

        builder
            .HasOne(x => x.HistoryTypeEntity)
            .WithMany(x => x.TransactionHistoryEntities)
            .HasForeignKey(x => x.HistoryTypeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(x => x.ProcessingStatusEntity)
            .WithMany(x => x.TransactionHistoryEntities)
            .HasForeignKey(x => x.ProcessingStatusId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(x => x.AccountEntity)
            .WithMany()
            .HasForeignKey(x => x.AccountId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(x => x.TransactionTypeEntity)
            .WithMany()
            .HasForeignKey(x => x.TransactionTypeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(x => x.TransactionSourceEntity)
            .WithMany()
            .HasForeignKey(x => x.TransactionSourceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(x => x.TransactionDirectionEntity)
            .WithMany()
            .HasForeignKey(x => x.TransactionDirectionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(x => x.TransactionStatusEntity)
            .WithMany()
            .HasForeignKey(x => x.TransactionStatusId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.ToTable(TableNames.TransactionHistory);
    }
}