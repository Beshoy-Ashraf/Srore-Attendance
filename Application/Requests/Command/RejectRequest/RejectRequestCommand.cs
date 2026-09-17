using MediatR;

namespace Application.Requests.Commands.RejectRequest;

public record RejectRequestCommand(Guid Id, Guid ApprovedByAreaManagerId) : IRequest;