using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class RequestConfiguration : IEntityTypeConfiguration<Request>
{
      public void Configure(EntityTypeBuilder<Request> builder)
      {
            builder.ToTable("Requests");
            builder.HasKey(r => r.Id);

            builder.Property(r => r.Type).HasConversion<string>().HasMaxLength(20);
            builder.Property(r => r.Status).HasConversion<string>().HasMaxLength(20);
            builder.Property(r => r.Reason).HasMaxLength(500);
            builder.Property(r => r.RejectionReason).HasMaxLength(500);

            builder.HasQueryFilter(r => r.DeletedDate == null);

            builder.HasOne(r => r.Staff)
                .WithMany()
                .HasForeignKey(r => r.StaffId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.RequestedBy)
                .WithMany()
                .HasForeignKey(r => r.RequestedById)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.ApprovedByAreaManager)
                .WithMany()
                .HasForeignKey(r => r.ApprovedByAreaManagerId)
                .OnDelete(DeleteBehavior.Restrict);
      }
}