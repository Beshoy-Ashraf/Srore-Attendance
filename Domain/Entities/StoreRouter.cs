namespace Domain.Entities;

public class StoreRouter
{
      public Guid Id { get; set; }
      public Guid StoreId { get; set; }

      public string MacAddress { get; set; } = default!;
      public string? Label { get; set; }

      public DateTime CreatedDate { get; set; }

      // Navigation
      public Store? Store { get; set; }
}