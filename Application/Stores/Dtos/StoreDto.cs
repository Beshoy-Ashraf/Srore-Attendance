namespace Application.Stores.Dtos;

public record StoreDto(
    Guid Id,
    string Name,
    List<string> RouterMacs,
    Guid? AreaManagerId);