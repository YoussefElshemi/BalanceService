using Infrastructure.Constants;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class TransactionTypeEntityConfiguration : IEntityTypeConfiguration<TransactionTypeEntity>
{
    public void Configure(EntityTypeBuilder<TransactionTypeEntity> builder)
    {
        builder.HasKey(x => x.TransactionTypeId);

        builder.HasData(SeedData.TransactionTypes);

        builder.ToTable(TableNames.TransactionTypes);
    }
}