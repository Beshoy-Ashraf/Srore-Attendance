namespace Domain.Entities;

public class AttendanceSettings
{
      public Guid Id { get; set; }
      public Guid StoreId { get; set; }

      public TimeOnly MorningStart { get; set; }
      public TimeOnly MorningEnd { get; set; }
      public TimeOnly NightStart { get; set; }
      public TimeOnly NightEnd { get; set; }
      public TimeOnly BetweenStart { get; set; }
      public TimeOnly BetweenEnd { get; set; }

      public int LateGraceMinutes { get; set; }

      public DateTime CreatedDate { get; set; }
      public DateTime? UpdateDate { get; set; }


      public Store Store { get; set; } = default!;
}