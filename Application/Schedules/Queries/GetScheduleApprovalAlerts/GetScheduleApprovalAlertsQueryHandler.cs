using Application.Common;
using Application.Common.Interfaces;
using Application.Schedules.Dtos;
using Domain.Interfaces;
using MediatR;

namespace Application.Schedules.Queries.GetScheduleApprovalAlerts;

public class GetScheduleApprovalAlertsQueryHandler(IUnitOfWork unitOfWork, IAccessService access, IClock clock)
    : IRequestHandler<GetScheduleApprovalAlertsQuery, IReadOnlyList<ScheduleApprovalAlertDto>>
{
      public async Task<IReadOnlyList<ScheduleApprovalAlertDto>> Handle(
          GetScheduleApprovalAlertsQuery request, CancellationToken cancellationToken)
      {
            var storeIds = await access.GetVisibleStoreIdsAsync(cancellationToken);
            var slices = await unitOfWork.ScheduleRepository.GetPendingSlicesAsync(storeIds, cancellationToken);

            var today = clock.LocalToday;
            var alertCutoff = today.AddDays(AttendanceRules.ScheduleAlertLeadDays);

            return slices
                .Where(s => new DateOnly(s.Date.Year, s.Date.Month, 1) <= new DateOnly(alertCutoff.Year, alertCutoff.Month, 1))
                .GroupBy(s => (s.StoreId, s.StoreName, s.Date.Year, s.Date.Month))
                .Select(g => new ScheduleApprovalAlertDto(
                    g.Key.StoreId,
                    g.Key.StoreName,
                    g.Key.Year,
                    g.Key.Month,
                    g.Count(),
                    MonthHasStarted: new DateOnly(g.Key.Year, g.Key.Month, 1) <= today))
                .OrderByDescending(a => a.MonthHasStarted)
                .ThenBy(a => a.Year).ThenBy(a => a.Month).ThenBy(a => a.StoreName)
                .ToList();
      }
}
