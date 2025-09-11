using Infrastructure.Constants;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class TransactionStatusEntityConfiguration : IEntityTypeConfiguration<TransactionStatusEntity>
{
    public void Configure(EntityTypeBuilder<TransactionStatusEntity> builder)
    {
        builder.HasKey(x => x.TransactionStatusId);

        builder.HasData(SeedData.TransactionStatuses);

        builder.ToTable(TableNames.TransactionStatuses);
    }
}