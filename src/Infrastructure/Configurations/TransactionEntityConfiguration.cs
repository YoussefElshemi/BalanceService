using Infrastructure.Constants;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class TransactionEntityConfiguration : IEntityTypeConfiguration<TransactionEntity>
{
    public void Configure(EntityTypeBuilder<TransactionEntity> builder)
    {
        builder.HasKey(x => x.TransactionId);
        builder.Property(x => x.RowVersion).IsRowVersion();

        builder.HasIndex(x => x.IdempotencyKey).IsUnique();

        builder
            .HasOne(x => x.AccountEntity)
            .WithMany(x => x.TransactionEntities)
            .HasForeignKey(x => x.AccountId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(x => x.TransactionTypeEntity)
            .WithMany(x => x.TransactionEntities)
            .HasForeignKey(x => x.TransactionTypeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(x => x.TransactionSourceEntity)
            .WithMany(x => x.TransactionEntities)
            .HasForeignKey(x => x.TransactionSourceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(x => x.TransactionDirectionEntity)
            .WithMany(x => x.TransactionEntities)
            .HasForeignKey(x => x.TransactionDirectionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(x => x.TransactionStatusEntity)
            .WithMany(x => x.TransactionEntities)
            .HasForeignKey(x => x.TransactionStatusId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.ToTable(TableNames.Transactions);
    }
}