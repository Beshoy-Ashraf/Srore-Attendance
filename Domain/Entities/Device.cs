namespace Domain.Entities;

public class Device
{
      public Guid Id { get; set; }
      public Guid StaffId { get; set; }

      public string RegisteredRouterMac { get; set; } = default!;
      public string RegisteredDeviceId { get; set; } = default!;
      public DateTime RegisteredDate { get; set; }
      public bool IsActive { get; set; } = true;

      // Navigation
      public User Staff { get; set; } = default!;
}