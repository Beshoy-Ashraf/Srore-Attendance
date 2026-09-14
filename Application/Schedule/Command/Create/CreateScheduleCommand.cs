using Domain.Enums;
using MediatR;

namespace Application.Schedules.Commands.CreateSchedule;

public record CreateScheduleCommand(
    Guid StaffId,
    Guid CreatedByStoreManagerId,
    DateOnly Date,
    ShiftType ShiftType,
    TimeOnly StartTime,
    TimeOnly EndTime) : IRequest<Guid>;