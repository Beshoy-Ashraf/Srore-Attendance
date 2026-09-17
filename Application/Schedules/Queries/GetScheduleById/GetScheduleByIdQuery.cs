using Application.Schedules.Dtos;
using MediatR;

namespace Application.Schedules.Queries.GetScheduleById;

public record GetScheduleByIdQuery(Guid Id) : IRequest<ScheduleDto>;