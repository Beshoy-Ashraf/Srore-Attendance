using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Schedules.Dtos;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Models;
using MediatR;

namespace Application.Schedules.Queries.GetPendingSchedules;

public class GetPendingSchedulesQueryHandler(IUnitOfWork unitOfWork, IAccessService access)
    : IRequestHandler<GetPendingSchedulesQuery, PagedResult<ScheduleDto>>
{
      public async Task<PagedResult<ScheduleDto>> Handle(GetPendingSchedulesQuery request, CancellationToken cancellationToken)
      {
            await access.EnsureRoleAsync(cancellationToken, UserRole.Admin, UserRole.AreaManager);
            var (page, pageSize) = Paging.Normalize(request.Page, request.PageSize);

            var storeIds = await access.ResolveStoreScopeAsync(request.StoreId, cancellationToken);

            var filter = new ScheduleFilter
            {
                  StoreIds = storeIds,
                  Status = ScheduleStatus.Pending,
                  Page = page,
                  PageSize = pageSize
            };

            var result = await unitOfWork.ScheduleRepository.GetPagedAsync(filter, cancellationToken);
            return PagedResult<ScheduleDto>.From(result, s => s.ToDto(), page, pageSize);
      }
}
