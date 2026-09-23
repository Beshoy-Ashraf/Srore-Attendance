using Domain.Enums;

namespace Application.Attendance.Dtos;

public record AttendanceDto(
    Guid Id,
    Guid StaffId,
    string? StaffName,
    Guid? StoreId,
    Guid? ScheduleId,
    DateTime? CheckInTime,
    DateTime? CheckOutTime,
    string? CheckInDeviceMac,
    string? CheckInIp,
    VerificationMethod VerificationMethod,
    bool IsLate,
    Guid? EnteredManuallyBy,
    string? EnteredManuallyByName,
    string? Notes);

public static class AttendanceMappings
{
    public static AttendanceDto ToDto(this Domain.Entities.Attendance a) => new(
        a.Id,
        a.StaffId,
        a.Staff?.DisplayName,
        a.Staff?.StoreId,
        a.ScheduleId,
        a.CheckInTime,
        a.CheckOutTime,
        a.CheckInDeviceMac,
        a.CheckInIp,
        a.VerificationMethod,
        a.IsLate,
        a.EnteredManuallyBy,
        a.EnteredManuallyByUser?.DisplayName,
        a.Notes);
}
