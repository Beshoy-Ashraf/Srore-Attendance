using Application.Schedules.Dtos;
using Domain.Enums;
using MediatR;

namespace Application.Schedules.Queries.GetSchedules;

public record GetSchedulesQuery(
    Guid? StaffId,
    Guid? StoreId,
    DateOnly? From,
    DateOnly? To,
    ScheduleStatus? Status,
    int Page = 1,
    int PageSize = 20) : IRequest<IEnumerable<ScheduleDto>>;