using Application.Common.Models;
using Application.Requests.Dtos;
using MediatR;

namespace Application.Requests.Queries.GetPendingRequests;

public record GetPendingRequestsQuery(
    Guid? StoreId,
    int Page = 1,
    int PageSize = 20) : IRequest<PagedResult<RequestDto>>;
