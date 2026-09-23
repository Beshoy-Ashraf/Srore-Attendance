using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Missions.Dtos;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Models;
using MediatR;

namespace Application.Missions.Queries.GetPendingMissions;

public class GetPendingMissionsQueryHandler(IUnitOfWork unitOfWork, IAccessService access)
    : IRequestHandler<GetPendingMissionsQuery, PagedResult<MissionDto>>
{
      public async Task<PagedResult<MissionDto>> Handle(GetPendingMissionsQuery request, CancellationToken cancellationToken)
      {
            await access.EnsureRoleAsync(cancellationToken, UserRole.Admin, UserRole.AreaManager);
            var (page, pageSize) = Paging.Normalize(request.Page, request.PageSize);

            var storeIds = await access.ResolveStoreScopeAsync(request.StoreId, cancellationToken);

            var filter = new MissionFilter
            {
                  StoreIds = storeIds,
                  Status = RequestStatus.Pending,
                  Page = page,
                  PageSize = pageSize
            };

            var result = await unitOfWork.MissionRepository.GetPagedAsync(filter, cancellationToken);
            return PagedResult<MissionDto>.From(result, m => m.ToDto(), page, pageSize);
      }
}
