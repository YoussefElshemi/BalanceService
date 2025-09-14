using Core.Enums;
using Infrastructure.Constants;
using Infrastructure.Entities.History;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class AccountHistoryEntityConfiguration : IEntityTypeConfiguration<AccountHistoryEntity>
{
    public void Configure(EntityTypeBuilder<AccountHistoryEntity> builder)
    {
        builder.HasKey(x => x.AccountHistoryId);
        builder.Property(x => x.RowVersion).IsRowVersion();
        builder.Property(x => x.ProcessingStatusId).HasDefaultValue((int)ProcessingStatus.NotProcessed);

        builder.HasIndex(x => x.AccountId);

        builder.Property(x => x.Metadata).HasColumnType("jsonb");

        builder
            .HasOne(x => x.HistoryTypeEntity)
            .WithMany(x => x.AccountHistoryEntities)
            .HasForeignKey(x => x.HistoryTypeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(x => x.ProcessingStatusEntity)
            .WithMany(x => x.AccountHistoryEntities)
            .HasForeignKey(x => x.ProcessingStatusId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(x => x.ParentAccountEntity)
            .WithMany()
            .HasForeignKey(x => x.ParentAccountId)
            .OnDelete(DeleteBehavior.SetNull);

        builder
            .HasOne(x => x.AccountTypeEntity)
            .WithMany()
            .HasForeignKey(x => x.AccountTypeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(x => x.AccountStatusEntity)
            .WithMany()
            .HasForeignKey(x => x.AccountStatusId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.ToTable(TableNames.AccountHistory);
    }
}