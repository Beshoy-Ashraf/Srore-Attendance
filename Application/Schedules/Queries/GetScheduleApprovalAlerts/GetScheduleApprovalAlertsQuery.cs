using Application.Schedules.Dtos;
using MediatR;

namespace Application.Schedules.Queries.GetScheduleApprovalAlerts;

/// <summary>Stores/months with schedules still pending approval, within AttendanceRules.ScheduleAlertLeadDays
/// of that month starting, or already inside it. Requirement: alert the Area Manager if a month arrives
/// without their having approved (or rejected) the schedule.</summary>
public record GetScheduleApprovalAlertsQuery : IRequest<IReadOnlyList<ScheduleApprovalAlertDto>>;
