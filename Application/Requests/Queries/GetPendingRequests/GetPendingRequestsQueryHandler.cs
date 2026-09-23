using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Requests.Dtos;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Models;
using MediatR;

namespace Application.Requests.Queries.GetPendingRequests;

public class GetPendingRequestsQueryHandler(IUnitOfWork unitOfWork, IAccessService access)
    : IRequestHandler<GetPendingRequestsQuery, PagedResult<RequestDto>>
{
      public async Task<PagedResult<RequestDto>> Handle(GetPendingRequestsQuery request, CancellationToken cancellationToken)
      {
            await access.EnsureRoleAsync(cancellationToken, UserRole.Admin, UserRole.AreaManager);
            var (page, pageSize) = Paging.Normalize(request.Page, request.PageSize);

            var storeIds = await access.ResolveStoreScopeAsync(request.StoreId, cancellationToken);

            var filter = new RequestFilter
            {
                  StoreIds = storeIds,
                  Status = RequestStatus.Pending,
                  Page = page,
                  PageSize = pageSize
            };

            var result = await unitOfWork.RequestRepository.GetPagedAsync(filter, cancellationToken);
            return PagedResult<RequestDto>.From(result, r => r.ToDto(), page, pageSize);
      }
}
