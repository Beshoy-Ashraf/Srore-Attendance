namespace Application.Stores.Dtos;

public record StoreDto(
    Guid Id,
    string Name,
    decimal Latitude,
    decimal Longitude,
    int GeofenceRadiusMeters,
    List<string> RouterMacs,
    Guid? AreaManagerId);