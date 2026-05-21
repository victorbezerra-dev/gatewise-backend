using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GateWise.Core.Entities;

public class AccessGrantConfiguration : IEntityTypeConfiguration<AccessGrant>
{
    public void Configure(EntityTypeBuilder<AccessGrant> builder)
    {
        builder.ToTable("access_grants");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Reason).HasMaxLength(255);

        builder.Property(a => a.Status)
               .HasConversion<string>()
               .IsRequired();

        builder.Property(a => a.AuthorizedUserId)
               .HasColumnType("varchar(36)")
               .IsRequired();

        builder.Property(a => a.GrantedByUserId)
               .HasColumnType("varchar(36)")
               .IsRequired(false);

        builder.HasOne(a => a.Space)
               .WithMany()
               .HasForeignKey(a => a.SpaceId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
