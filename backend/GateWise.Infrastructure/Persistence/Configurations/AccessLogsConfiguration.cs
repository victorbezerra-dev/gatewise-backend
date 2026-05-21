using GateWise.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class AccessLogConfiguration : IEntityTypeConfiguration<AccessLog>
{
    public void Configure(EntityTypeBuilder<AccessLog> builder)
    {
        builder.ToTable("access_logs");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Status)
               .HasConversion<string>()
               .IsRequired();

        builder.Property(a => a.IssuedAt).IsRequired();

        builder.Property(a => a.CreatedAt)
               .HasDefaultValueSql("CURRENT_TIMESTAMP")
               .ValueGeneratedOnAdd();

        builder.Property(a => a.UpdatedAt)
               .HasDefaultValueSql("CURRENT_TIMESTAMP")
               .ValueGeneratedOnAddOrUpdate();

        builder.Property(a => a.RawRequestJson).IsRequired();
        builder.Property(a => a.CommandId).IsRequired();
        builder.Property(a => a.SpaceId).IsRequired();

        builder.HasOne(a => a.Space)
               .WithMany()
               .HasForeignKey(a => a.SpaceId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.Property(a => a.UserId).IsRequired();

        builder.HasOne(a => a.User)
               .WithMany()
               .HasForeignKey(a => a.UserId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
