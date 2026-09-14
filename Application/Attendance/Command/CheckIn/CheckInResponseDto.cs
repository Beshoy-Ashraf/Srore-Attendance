namespace Application.Attendance.Commands.CheckIn;

public record CheckInResponseDto(
    Guid AttendanceId,
    DateTime CheckInTime,
    bool IsLate,
    string Message);