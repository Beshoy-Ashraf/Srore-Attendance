using Domain.Enums;

namespace Application.Schedules.Dtos;

public record ScheduleDto(
    Guid Id,
    Guid StaffId,
    string? StaffName,
    Guid? StoreId,
    DateOnly Date,
    ShiftType ShiftType,
    TimeOnly StartTime,
    TimeOnly EndTime,
    ScheduleStatus Status,
    Guid CreatedByStoreManagerId,
    Guid? ApprovedByAreaManagerId,
    string? ApprovedByAreaManagerName,
    DateTime? ApprovedDate,
    string? RejectionReason);

public static class ScheduleMappings
{
    public static ScheduleDto ToDto(this Domain.Entities.Schedule s) => new(
        s.Id,
        s.StaffId,
        s.Staff?.DisplayName,
        s.Staff?.StoreId,
        s.Date,
        s.ShiftType,
        s.StartTime,
        s.EndTime,
        s.Status,
        s.CreatedByStoreManagerId,
        s.ApprovedByAreaManagerId,
        s.ApprovedByAreaManager?.DisplayName,
        s.ApprovedDate,
        s.RejectionReason);
}
