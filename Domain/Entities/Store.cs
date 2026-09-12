namespace Domain.Entities;

public class Store
{
      public Guid Id { get; set; }
      public string Name { get; set; } = default!;
      public string StoreID { get; set; } = default!;

      public decimal Latitude { get; set; }
      public decimal Longitude { get; set; }
      public int GeofenceRadiusMeters { get; set; }
      public string RouterMac { get; set; } = default!;

      public Guid? AreaManagerId { get; set; }

      public DateTime CreatedDate { get; set; }
      public DateTime? UpdateDate { get; set; }
      public DateTime? DeleteDate { get; set; }

      public User? AreaManager { get; set; }
      public AttendanceSettings? AttendanceSettings { get; set; }
      public ICollection<User> Staff { get; set; } = new List<User>();
}