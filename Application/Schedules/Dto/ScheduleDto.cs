using Domain.Enums;

namespace Application.Schedules.Dtos;

public record ScheduleDto(
    Guid Id,
    Guid StaffId,
    DateOnly Date,
    ShiftType ShiftType,
    TimeOnly StartTime,
    TimeOnly EndTime,
    ScheduleStatus Status,
    Guid CreatedByStoreManagerId,
    Guid? ApprovedByAreaManagerId,
    DateTime? ApprovedDate);