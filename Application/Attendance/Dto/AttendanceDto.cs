using Domain.Enums;

namespace Application.Attendance.Dtos;

public record AttendanceDto(
    Guid Id,
    Guid StaffId,
    Guid? ScheduleId,
    DateTime? CheckInTime,
    DateTime? CheckOutTime,
    string? CheckInRouterMac,
    string? CheckInDeviceMac,
    string? CheckInIp,
    VerificationMethod VerificationMethod,
    bool IsLate,
    Guid? EnteredManuallyBy,
    string? Notes);