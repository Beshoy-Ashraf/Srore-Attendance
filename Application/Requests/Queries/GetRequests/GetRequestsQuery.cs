using Application.Requests.Dtos;
using Domain.Enums;
using MediatR;

namespace Application.Requests.Queries.GetRequests;

public record GetRequestsQuery(
    Guid? StaffId,
    RequestType? Type,
    RequestStatus? Status,
    int Page = 1,
    int PageSize = 20) : IRequest<IEnumerable<RequestDto>>;