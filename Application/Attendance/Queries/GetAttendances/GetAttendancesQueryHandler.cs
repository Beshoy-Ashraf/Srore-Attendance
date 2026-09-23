using Application.Attendance.Dtos;
using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Interfaces;
using Domain.Models;
using MediatR;

namespace Application.Attendance.Queries.GetAttendances;

/// <summary>Requirement: Area Managers and Store Managers see attendance reports — scoped to the stores
/// they can see, never global, via IAccessService.</summary>
public class GetAttendancesQueryHandler(IUnitOfWork unitOfWork, IAccessService access)
    : IRequestHandler<GetAttendancesQuery, PagedResult<AttendanceDto>>
{
      public async Task<PagedResult<AttendanceDto>> Handle(GetAttendancesQuery request, CancellationToken cancellationToken)
      {
            var (page, pageSize) = Paging.Normalize(request.Page, request.PageSize);

            if (request.StaffId is { } staffId)
                  await access.EnsureStaffAccessAsync(staffId, cancellationToken);

            var storeIds = await access.ResolveStoreScopeAsync(request.StoreId, cancellationToken);

            var filter = new AttendanceFilter
            {
                  StaffId = request.StaffId,
                  StoreIds = storeIds,
                  FromUtc = request.From,
                  ToUtcExclusive = request.To,
                  LateOnly = request.LateOnly,
                  Page = page,
                  PageSize = pageSize
            };

            var result = await unitOfWork.AttendanceRepository.GetPagedAsync(filter, cancellationToken);
            return PagedResult<AttendanceDto>.From(result, a => a.ToDto(), page, pageSize);
      }
}
