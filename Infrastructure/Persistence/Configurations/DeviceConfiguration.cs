using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class DeviceConfiguration : IEntityTypeConfiguration<Device>
{
      public void Configure(EntityTypeBuilder<Device> builder)
      {
            builder.ToTable("Devices");
            builder.HasKey(d => d.Id);

            builder.Property(d => d.RegisteredRouterMac).IsRequired().HasMaxLength(17);
            builder.Property(d => d.RegisteredDeviceId).IsRequired().HasMaxLength(200);

            builder.HasOne(d => d.Staff)
                .WithOne()
                .HasForeignKey<Device>(d => d.StaffId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(d => d.StaffId).IsUnique();
      }
}