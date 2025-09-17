using Infrastructure.Constants;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class JobEntityConfiguration : IEntityTypeConfiguration<JobEntity>
{
    public void Configure(EntityTypeBuilder<JobEntity> builder)
    {
        builder.HasKey(x => x.JobId);
        builder.Property(x => x.RowVersion).IsRowVersion();
        builder.HasIndex(x => x.JobName).IsUnique().HasFilter($"\"{nameof(JobEntity.IsDeleted)}\" = FALSE");

        builder.ToTable(TableNames.Jobs);
    }
}