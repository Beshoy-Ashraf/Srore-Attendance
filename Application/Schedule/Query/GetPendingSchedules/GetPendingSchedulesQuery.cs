using Application.Schedules.Dtos;
using MediatR;

namespace Application.Schedules.Queries.GetPendingSchedules;

public record GetPendingSchedulesQuery(Guid AreaManagerId) : IRequest<IEnumerable<ScheduleDto>>;