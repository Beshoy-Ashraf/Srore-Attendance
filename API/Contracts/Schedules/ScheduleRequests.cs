using Domain.Enums;

namespace API.Contracts.Schedules;

public record CreateScheduleRequest(
    Guid StaffId,
    DateOnly Date,
    ShiftType ShiftType,
    TimeOnly StartTime,
    TimeOnly EndTime);

public record UpdateScheduleRequest(
    ShiftType ShiftType,
    TimeOnly StartTime,
    TimeOnly EndTime);