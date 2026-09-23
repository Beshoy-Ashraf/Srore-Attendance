using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Missions.Dtos;
using Domain.Interfaces;
using Domain.Models;
using MediatR;

namespace Application.Missions.Queries.GetMissions;

public class GetMissionsQueryHandler(IUnitOfWork unitOfWork, IAccessService access)
    : IRequestHandler<GetMissionsQuery, PagedResult<MissionDto>>
{
      public async Task<PagedResult<MissionDto>> Handle(GetMissionsQuery request, CancellationToken cancellationToken)
      {
            var (page, pageSize) = Paging.Normalize(request.Page, request.PageSize);

            if (request.StaffId is { } staffId)
                  await access.EnsureStaffAccessAsync(staffId, cancellationToken);

            var storeIds = await access.ResolveStoreScopeAsync(request.StoreId, cancellationToken);

            var filter = new MissionFilter
            {
                  StaffId = request.StaffId,
                  StoreIds = storeIds,
                  Status = request.Status,
                  Page = page,
                  PageSize = pageSize
            };

            var result = await unitOfWork.MissionRepository.GetPagedAsync(filter, cancellationToken);
            return PagedResult<MissionDto>.From(result, m => m.ToDto(), page, pageSize);
      }
}
