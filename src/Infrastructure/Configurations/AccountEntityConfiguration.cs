using Infrastructure.Constants;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class AccountEntityConfiguration : IEntityTypeConfiguration<AccountEntity>
{
    public void Configure(EntityTypeBuilder<AccountEntity> builder)
    {
        builder.HasKey(x => x.AccountId);
        builder.Property(x => x.RowVersion).IsRowVersion();
        builder.Property(x => x.Metadata).HasColumnType("jsonb");

        builder
            .HasOne(x => x.ParentAccountEntity)
            .WithMany(x => x.ChildAccountEntities)
            .HasForeignKey(x => x.ParentAccountId)
            .OnDelete(DeleteBehavior.SetNull);

        builder
            .HasOne(x => x.AccountTypeEntity)
            .WithMany(x => x.AccountEntities)
            .HasForeignKey(x => x.AccountTypeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(x => x.AccountStatusEntity)
            .WithMany(x => x.AccountEntities)
            .HasForeignKey(x => x.AccountStatusId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.ToTable(TableNames.Accounts);
    }
}