using Domain.Enums;

namespace Domain.Entities;

public class Attendance
{
      public Guid Id { get; set; }
      public Guid StaffId { get; set; }
      public Guid? ScheduleId { get; set; }

      public DateTime? CheckInTime { get; set; }
      public DateTime? CheckOutTime { get; set; }

      public string? CheckInRouterMac { get; set; }
      public string? CheckInDeviceMac { get; set; }
      public string? CheckInIp { get; set; }

      public VerificationMethod VerificationMethod { get; set; }
      public bool IsLate { get; set; }
      public bool IsDeviceMismatch { get; set; }

      public Guid? EnteredManuallyBy { get; set; }
      public string? Notes { get; set; }

      public DateTime CreatedDate { get; set; }
      public DateTime? UpdateDate { get; set; }

      // Navigation
      public User Staff { get; set; } = default!;
      public Schedule? Schedule { get; set; }
      public User? EnteredManuallyByUser { get; set; }
}