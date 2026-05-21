using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GateWise.Core.Entities;

public class SpaceConfiguration : IEntityTypeConfiguration<Space>
{
    public void Configure(EntityTypeBuilder<Space> builder)
    {
        builder.ToTable("spaces");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name).IsRequired().HasMaxLength(100);
        builder.Property(s => s.Code).IsRequired().HasMaxLength(50);
        builder.Property(s => s.ImageUrl).HasMaxLength(255);
        builder.Property(s => s.Description).HasMaxLength(500);
        builder.Property(s => s.Location).HasMaxLength(100);
        builder.Property(s => s.Building).HasMaxLength(100);

        builder.Property(s => s.OpenTime).IsRequired();
        builder.Property(s => s.CloseTime).IsRequired();
        builder.Property(s => s.IsActive).IsRequired();

        builder.Property(s => s.CreatedAt).IsRequired();
        builder.Property(s => s.UpdatedAt).IsRequired();
    }
}
