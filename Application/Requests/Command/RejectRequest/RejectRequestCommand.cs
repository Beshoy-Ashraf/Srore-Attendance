using MediatR;

namespace Application.Requests.Commands.RejectRequest;

public record RejectRequestCommand(Guid Id, string Reason) : IRequest;
