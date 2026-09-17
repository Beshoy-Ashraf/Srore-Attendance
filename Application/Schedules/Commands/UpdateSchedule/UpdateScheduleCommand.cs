using Domain.Enums;
using MediatR;

namespace Application.Schedules.Commands.UpdateSchedule;

public record UpdateScheduleCommand(
    Guid Id,
    ShiftType ShiftType,
    TimeOnly StartTime,
    TimeOnly EndTime) : IRequest;