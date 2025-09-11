using Infrastructure.Constants;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class TransactionSourceEntityConfiguration : IEntityTypeConfiguration<TransactionSourceEntity>
{
    public void Configure(EntityTypeBuilder<TransactionSourceEntity> builder)
    {
        builder.HasKey(x => x.TransactionSourceId);

        builder.HasData(SeedData.TransactionSources);

        builder.ToTable(TableNames.TransactionSources);
    }
}