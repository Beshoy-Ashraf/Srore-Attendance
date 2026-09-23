using Application.Common.Models;
using Application.Schedules.Dtos;
using MediatR;

namespace Application.Schedules.Queries.GetPendingSchedules;

/// <summary>The Area Manager's (or Admin's) approval queue: pending schedules across their visible stores.</summary>
public record GetPendingSchedulesQuery(
    Guid? StoreId,
    int Page = 1,
    int PageSize = 20) : IRequest<PagedResult<ScheduleDto>>;
