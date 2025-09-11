using Infrastructure.Constants;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class HoldSourceEntityConfiguration : IEntityTypeConfiguration<HoldSourceEntity>
{
    public void Configure(EntityTypeBuilder<HoldSourceEntity> builder)
    {
        builder.HasKey(x => x.HoldSourceId);

        builder.HasData(SeedData.HoldSources);

        builder.ToTable(TableNames.HoldSources);
    }
}