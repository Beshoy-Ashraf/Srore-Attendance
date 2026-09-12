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
            builder.Property(a => a.CheckInLatitude).HasColumnType("decimal(9,6)");
            builder.Property(a => a.CheckInLongitude).HasColumnType("decimal(9,6)");
            builder.Property(a => a.CheckInRouterMac).HasMaxLength(17);

            builder.HasOne(a => a.Staff)
                .WithMany()
                .HasForeignKey(a => a.StaffId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.EnteredManuallyByUser)
                .WithMany()
                .HasForeignKey(a => a.EnteredManuallyBy)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(a => new { a.StaffId, a.CheckInTime });
      }
}