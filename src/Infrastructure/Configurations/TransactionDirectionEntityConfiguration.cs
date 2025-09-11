using Infrastructure.Constants;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class TransactionDirectionEntityConfiguration : IEntityTypeConfiguration<TransactionDirectionEntity>
{
    public void Configure(EntityTypeBuilder<TransactionDirectionEntity> builder)
    {
        builder.HasKey(x => x.TransactionDirectionId);

        builder.HasData(SeedData.TransactionDirections);

        builder.ToTable(TableNames.TransactionDirections);
    }
}