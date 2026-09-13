using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class StoreConfiguration : IEntityTypeConfiguration<Store>
{
    public void Configure(EntityTypeBuilder<Store> builder)
    {
        builder.ToTable("Stores");
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name).IsRequired().HasMaxLength(150);

        builder.HasOne(s => s.AreaManager)
            .WithMany()
            .HasForeignKey(s => s.AreaManagerId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(s => s.AttendanceSettings)
            .WithOne(a => a.Store)
            .HasForeignKey<AttendanceSettings>(a => a.StoreId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(s => s.RouterMacs)
            .WithOne(r => r.Store)
            .HasForeignKey(r => r.StoreId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(s => s.Staff)
            .WithOne(u => u.Store)
            .HasForeignKey(u => u.StoreId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasQueryFilter(s => s.DeleteDate == null);
    }
}