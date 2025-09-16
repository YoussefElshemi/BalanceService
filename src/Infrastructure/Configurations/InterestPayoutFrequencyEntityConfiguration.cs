using Infrastructure.Constants;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class InterestPayoutFrequencyEntityConfiguration : IEntityTypeConfiguration<InterestPayoutFrequencyEntity>
{
    public void Configure(EntityTypeBuilder<InterestPayoutFrequencyEntity> builder)
    {
        builder.HasKey(x => x.InterestPayoutFrequencyId);

        builder.HasData(SeedData.InterestPayoutFrequencies);

        builder.ToTable(TableNames.InterestPayoutFrequencies);
    }
}