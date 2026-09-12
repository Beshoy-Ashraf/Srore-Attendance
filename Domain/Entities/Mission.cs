using Domain.Enums;

namespace Domain.Entities;

public class Mission
{
      public Guid Id { get; set; }
      public Guid StaffId { get; set; }

      public string Reason { get; set; } = default!;
      public DateOnly DateFrom { get; set; }
      public DateOnly DateTo { get; set; }
      public RequestStatus Status { get; set; } = RequestStatus.Pending;

      public Guid? ApprovedByAreaManagerId { get; set; }
      public DateTime? ApprovedDate { get; set; }

      public DateTime CreatedDate { get; set; }
      public DateTime? UpdateDate { get; set; }

      // Navigation
      public User Staff { get; set; } = default!;
      public User? ApprovedByAreaManager { get; set; }
}