using Infrastructure.Constants;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class HoldTypeEntityConfiguration : IEntityTypeConfiguration<HoldTypeEntity>
{
    public void Configure(EntityTypeBuilder<HoldTypeEntity> builder)
    {
        builder.HasKey(x => x.HoldTypeId);

        builder.HasData(SeedData.HoldTypes);

        builder.ToTable(TableNames.HoldTypes);
    }
}