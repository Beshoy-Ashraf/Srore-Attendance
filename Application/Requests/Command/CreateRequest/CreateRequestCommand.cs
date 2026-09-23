using Domain.Enums;
using MediatR;

namespace Application.Requests.Commands.CreateRequest;

public record CreateRequestCommand(
    Guid StaffId,
    RequestType Type,
    DateOnly DateFrom,
    DateOnly DateTo,
    string Reason) : IRequest<Guid>;
