namespace Domain.Entities;

public class Store
{
      public Guid Id { get; set; }
      public string Name { get; set; } = default!;
      public Guid? AreaManagerId { get; set; }

      public DateTime CreatedDate { get; set; }
      public DateTime? UpdateDate { get; set; }
      public DateTime? DeleteDate { get; set; }

      // Navigation
      public User? AreaManager { get; set; }
      public AttendanceSettings? AttendanceSettings { get; set; }
      public ICollection<StoreRouter> RouterMacs { get; set; } = new List<StoreRouter>();
      public ICollection<User> Staff { get; set; } = new List<User>();
}