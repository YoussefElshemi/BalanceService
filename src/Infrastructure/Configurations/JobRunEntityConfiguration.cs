using Infrastructure.Constants;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class JobRunEntityConfiguration : IEntityTypeConfiguration<JobRunEntity>
{
    public void Configure(EntityTypeBuilder<JobRunEntity> builder)
    {
        builder.HasKey(x => x.JobRunId);
        builder.Property(x => x.RowVersion).IsRowVersion();
        builder.HasIndex(x => new { x.JobId, x.ScheduledAt }).IsUnique().HasFilter($"\"{nameof(JobRunEntity.IsDeleted)}\" = FALSE");

        builder
            .HasOne(x => x.JobEntity)
            .WithMany(x => x.JobRunEntities)
            .HasForeignKey(x => x.JobId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.ToTable(TableNames.JobRuns);
    }
}