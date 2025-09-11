using Infrastructure.Constants;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class AccountTypeEntityConfiguration : IEntityTypeConfiguration<AccountTypeEntity>
{
    public void Configure(EntityTypeBuilder<AccountTypeEntity> builder)
    {
        builder.HasKey(x => x.AccountTypeId);

        builder.HasData(SeedData.AccountTypes);

        builder.ToTable(TableNames.AccountTypes);
    }
}