using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GateWise.Core.Entities;

public class SpaceManagerConfiguration : IEntityTypeConfiguration<SpaceManager>
{
    public void Configure(EntityTypeBuilder<SpaceManager> builder)
    {
        builder.ToTable("space_managers");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.UserId)
               .HasColumnType("varchar(36)")
               .IsRequired();

        builder.HasOne(s => s.Space)
               .WithMany()
               .HasForeignKey(s => s.SpaceId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(s => s.User)
               .WithMany()
               .HasForeignKey(s => s.UserId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
