using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Requests.Dtos;
using Domain.Interfaces;
using Domain.Models;
using MediatR;

namespace Application.Requests.Queries.GetRequests;

public class GetRequestsQueryHandler(IUnitOfWork unitOfWork, IAccessService access)
    : IRequestHandler<GetRequestsQuery, PagedResult<RequestDto>>
{
      public async Task<PagedResult<RequestDto>> Handle(GetRequestsQuery request, CancellationToken cancellationToken)
      {
            var (page, pageSize) = Paging.Normalize(request.Page, request.PageSize);

            if (request.StaffId is { } staffId)
                  await access.EnsureStaffAccessAsync(staffId, cancellationToken);

            var storeIds = await access.ResolveStoreScopeAsync(request.StoreId, cancellationToken);

            var filter = new RequestFilter
            {
                  StaffId = request.StaffId,
                  StoreIds = storeIds,
                  Type = request.Type,
                  Status = request.Status,
                  Page = page,
                  PageSize = pageSize
            };

            var result = await unitOfWork.RequestRepository.GetPagedAsync(filter, cancellationToken);
            return PagedResult<RequestDto>.From(result, r => r.ToDto(), page, pageSize);
      }
}
