using Infrastructure.Constants;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class AccountStatusEntityConfiguration : IEntityTypeConfiguration<AccountStatusEntity>
{
    public void Configure(EntityTypeBuilder<AccountStatusEntity> builder)
    {
        builder.HasKey(x => x.AccountStatusId);

        builder.HasData(SeedData.AccountStatuses);

        builder.ToTable(TableNames.AccountStatuses);
    }
}