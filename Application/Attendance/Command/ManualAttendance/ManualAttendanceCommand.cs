using MediatR;

namespace Application.Attendance.Commands.ManualAttendance;

public record ManualAttendanceCommand(
    Guid StaffId,
    Guid EnteredManuallyBy,
    DateOnly Date,
    TimeOnly? CheckInTime,
    TimeOnly? CheckOutTime,
    string? Notes) : IRequest<Guid>;