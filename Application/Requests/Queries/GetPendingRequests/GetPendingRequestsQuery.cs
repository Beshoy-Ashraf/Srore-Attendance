using Application.Requests.Dtos;
using MediatR;

namespace Application.Requests.Queries.GetPendingRequests;

public record GetPendingRequestsQuery(Guid AreaManagerId) : IRequest<IEnumerable<RequestDto>>;