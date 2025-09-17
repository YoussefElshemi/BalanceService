using Infrastructure.Constants;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class InterestProductAccountLinkEntityConfiguration : IEntityTypeConfiguration<InterestProductAccountLinkEntity>
{
    public void Configure(EntityTypeBuilder<InterestProductAccountLinkEntity> builder)
    {
        builder.HasKey(x => new { x.AccountId, x.InterestProductId });
        builder.Property(x => x.RowVersion).IsRowVersion();
        builder.HasIndex(x => x.AccountId).IsUnique().HasFilter($"\"{nameof(InterestProductAccountLinkEntity.IsDeleted)}\" = FALSE"); ;

        builder
            .HasOne(x => x.AccountEntity)
            .WithMany(x => x.InterestProductAccountLinkEntities)
            .HasForeignKey(x => x.AccountId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(x => x.InterestProductEntity)
            .WithMany(x => x.InterestProductAccountLinkEntities)
            .HasForeignKey(x => x.InterestProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.ToTable(TableNames.InterestProductAccountLinks);
    }
}