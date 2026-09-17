using MediatR;

namespace Application.Requests.Commands.ApproveRequest;

public record ApproveRequestCommand(Guid Id, Guid ApprovedByAreaManagerId) : IRequest;