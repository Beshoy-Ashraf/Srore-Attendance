namespace Application.Attendance.Commands.CheckOut;

public record CheckOutResponseDto(
    Guid AttendanceId,
    DateTime CheckOutTime,
    TimeSpan HoursWorked,
    string Message);