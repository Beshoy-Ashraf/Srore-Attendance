using MediatR;

namespace Application.Stores.Commands.CreateStore;

public record CreateStoreCommand(
    string Name,
    decimal Latitude,
    decimal Longitude,
    int GeofenceRadiusMeters,
    List<string> RouterMacs,
    Guid? AreaManagerId) : IRequest<Guid>;