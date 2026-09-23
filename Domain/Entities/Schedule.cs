
using Domain.Enums;

namespace Domain.Entities;

public class Schedule
{
      public Guid Id { get; set; }
      public Guid StaffId { get; set; }

      public DateOnly Date { get; set; }
      public ShiftType ShiftType { get; set; }
      public TimeOnly StartTime { get; set; }
      public TimeOnly EndTime { get; set; }
      public ScheduleStatus Status { get; set; } = ScheduleStatus.Pending;

      public Guid CreatedByStoreManagerId { get; set; }
      public Guid? ApprovedByAreaManagerId { get; set; }
      public DateTime? ApprovedDate { get; set; }

      /// <summary>Why the Area Manager rejected the schedule. A rejected schedule is soft-deleted.</summary>
      public string? RejectionReason { get; set; }

      public DateTime CreatedDate { get; set; }
      public DateTime? UpdateDate { get; set; }
      public DateTime? DeletedDate { get; set; }

      // Navigation
      public User Staff { get; set; } = default!;
      public User CreatedByStoreManager { get; set; } = default!;
      public User? ApprovedByAreaManager { get; set; }
      public Attendance? Attendance { get; set; }
}