using Infrastructure.Constants;
using Infrastructure.Entities;
using Infrastructure.Entities.History;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class HistoryTypeEntityConfiguration : IEntityTypeConfiguration<HistoryTypeEntity>
{
    public void Configure(EntityTypeBuilder<HistoryTypeEntity> builder)
    {
        builder.HasKey(x => x.HistoryTypeId);

        builder.HasData(SeedData.HistoryTypes);

        builder.ToTable(TableNames.HistoryTypes);
    }
}