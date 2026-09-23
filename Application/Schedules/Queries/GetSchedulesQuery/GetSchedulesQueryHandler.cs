using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Schedules.Dtos;
using Domain.Interfaces;
using Domain.Models;
using MediatR;

namespace Application.Schedules.Queries.GetSchedules;

/// <summary>Status=Rejected returns the Area Manager's rejected-schedule history (soft-deleted rows) — see
/// IScheduleRepository.GetPagedAsync. Every other status only ever sees live rows.</summary>
public class GetSchedulesQueryHandler(IUnitOfWork unitOfWork, IAccessService access)
    : IRequestHandler<GetSchedulesQuery, PagedResult<ScheduleDto>>
{
      public async Task<PagedResult<ScheduleDto>> Handle(GetSchedulesQuery request, CancellationToken cancellationToken)
      {
            var (page, pageSize) = Paging.Normalize(request.Page, request.PageSize);

            if (request.StaffId is { } staffId)
                  await access.EnsureStaffAccessAsync(staffId, cancellationToken);

            var storeIds = await access.ResolveStoreScopeAsync(request.StoreId, cancellationToken);

            var filter = new ScheduleFilter
            {
                  StaffId = request.StaffId,
                  StoreIds = storeIds,
                  From = request.From,
                  To = request.To,
                  Status = request.Status,
                  Page = page,
                  PageSize = pageSize
            };

            var result = await unitOfWork.ScheduleRepository.GetPagedAsync(filter, cancellationToken);
            return PagedResult<ScheduleDto>.From(result, s => s.ToDto(), page, pageSize);
      }
}
