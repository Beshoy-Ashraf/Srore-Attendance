using Domain.Enums;

namespace Domain.Entities;

public class Request
{
      public Guid Id { get; set; }
      public Guid StaffId { get; set; }

      public RequestType Type { get; set; }
      public DateOnly DateFrom { get; set; }
      public DateOnly DateTo { get; set; }
      public string Reason { get; set; } = default!;
      public RequestStatus Status { get; set; } = RequestStatus.Pending;

      public Guid RequestedById { get; set; }
      public Guid? ApprovedByAreaManagerId { get; set; }
      public DateTime? ApprovedDate { get; set; }
      public string? RejectionReason { get; set; }

      public DateTime CreatedDate { get; set; }
      public DateTime? UpdateDate { get; set; }
      public DateTime? DeletedDate { get; set; }

      // Navigation
      public User Staff { get; set; } = default!;
      public User RequestedBy { get; set; } = default!;
      public User? ApprovedByAreaManager { get; set; }
}