using MediatR;

namespace Application.Stores.Commands.CreateStore;

public record CreateStoreCommand(
    string Name,
    List<string> DeviceMacs,
    Guid? AreaManagerId) : IRequest<Guid>;
