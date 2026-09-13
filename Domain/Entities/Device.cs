namespace Domain.Entities;

public class Device
{
      public Guid Id { get; set; }
      public Guid StaffId { get; set; }

      public string RegisteredDeviceMac { get; set; } = default!;
      public string? RegisteredDeviceIp { get; set; }
      public DateTime RegisteredDate { get; set; }
      public bool IsActive { get; set; } = true;

      // Navigation
      public User Staff { get; set; } = default!;
}