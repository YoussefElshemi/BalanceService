using Infrastructure.Constants;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class HoldStatusEntityConfiguration : IEntityTypeConfiguration<HoldStatusEntity>
{
    public void Configure(EntityTypeBuilder<HoldStatusEntity> builder)
    {
        builder.HasKey(x => x.HoldStatusId);

        builder.HasData(SeedData.HoldStatuses);

        builder.ToTable(TableNames.HoldStatuses);
    }
}