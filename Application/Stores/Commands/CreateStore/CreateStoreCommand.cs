using MediatR;

namespace Application.Stores.Commands.CreateStore;

public record CreateStoreCommand(
    string Name,
    List<string> RouterMacs,
    Guid? AreaManagerId) : IRequest<Guid>;