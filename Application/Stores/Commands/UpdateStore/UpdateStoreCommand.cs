using MediatR;

namespace Application.Stores.Commands.UpdateStore;

public record UpdateStoreCommand(
    Guid Id,
    string Name,
    List<string> RouterMacs,
    Guid? AreaManagerId) : IRequest;