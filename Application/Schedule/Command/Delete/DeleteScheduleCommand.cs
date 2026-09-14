using MediatR;

namespace Application.Schedules.Commands.DeleteSchedule;

public record DeleteScheduleCommand(Guid Id) : IRequest;