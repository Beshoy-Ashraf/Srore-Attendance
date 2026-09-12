using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class MissionConfiguration : IEntityTypeConfiguration<Mission>
{
      public void Configure(EntityTypeBuilder<Mission> builder)
      {
            builder.ToTable("Missions");
            builder.HasKey(m => m.Id);

            builder.Property(m => m.Status).HasConversion<string>().HasMaxLength(20);
            builder.Property(m => m.Reason).HasMaxLength(500);

            builder.HasOne(m => m.Staff)
                .WithMany()
                .HasForeignKey(m => m.StaffId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(m => m.ApprovedByAreaManager)
                .WithMany()
                .HasForeignKey(m => m.ApprovedByAreaManagerId)
                .OnDelete(DeleteBehavior.Restrict);
      }
}