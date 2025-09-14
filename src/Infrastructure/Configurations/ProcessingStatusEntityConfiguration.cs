using Infrastructure.Constants;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class ProcessingStatusEntityConfiguration : IEntityTypeConfiguration<ProcessingStatusEntity>
{
    public void Configure(EntityTypeBuilder<ProcessingStatusEntity> builder)
    {
        builder.HasKey(x => x.ProcessingStatusId);

        builder.HasData(SeedData.ProcessingStatuses);

        builder.ToTable(TableNames.ProcessingStatuses);
    }
}