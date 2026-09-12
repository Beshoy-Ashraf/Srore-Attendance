using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class ScheduleConfiguration : IEntityTypeConfiguration<Schedule>
{
      public void Configure(EntityTypeBuilder<Schedule> builder)
      {
            builder.ToTable("Schedules");
            builder.HasKey(s => s.Id);

            builder.Property(s => s.ShiftType).HasConversion<string>().HasMaxLength(10);
            builder.Property(s => s.Status).HasConversion<string>().HasMaxLength(20);

            builder.HasOne(s => s.Staff)
                .WithMany()
                .HasForeignKey(s => s.StaffId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(s => s.CreatedByStoreManager)
                .WithMany()
                .HasForeignKey(s => s.CreatedByStoreManagerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(s => s.ApprovedByAreaManager)
                .WithMany()
                .HasForeignKey(s => s.ApprovedByAreaManagerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(s => s.Attendance)
                .WithOne(a => a.Schedule)
                .HasForeignKey<Attendance>(a => a.ScheduleId)
                .OnDelete(DeleteBehavior.SetNull);

            // One schedule per staff member per day
            builder.HasIndex(s => new { s.StaffId, s.Date }).IsUnique();
      }
}