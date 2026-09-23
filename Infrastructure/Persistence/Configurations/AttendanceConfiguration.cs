using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class AttendanceConfiguration : IEntityTypeConfiguration<Attendance>
{
    public void Configure(EntityTypeBuilder<Attendance> builder)
    {
        builder.ToTable("Attendances");
        builder.HasKey(a => a.Id);

        builder.Property(a => a.VerificationMethod).HasConversion<string>().HasMaxLength(10);
        builder.Property(a => a.CheckInDeviceMac).HasMaxLength(17);
        builder.Property(a => a.CheckInIp).HasMaxLength(45); // IPv6-safe length

        builder.HasOne(a => a.Staff)
            .WithMany()
            .HasForeignKey(a => a.StaffId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.EnteredManuallyByUser)
            .WithMany()
            .HasForeignKey(a => a.EnteredManuallyBy)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(a => a.Notes).HasMaxLength(500);

        // Soft delete: a deleted attendance record disappears from every query but stays for audit.
        builder.HasQueryFilter(a => a.DeletedDate == null);

        builder.HasIndex(a => new { a.StaffId, a.CheckInTime });
    }
}