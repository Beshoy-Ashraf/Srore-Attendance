using MediatR;

namespace Application.Attendance.Commands.ManualAttendance;

public record ManualAttendanceCommand(
    Guid StaffId,
    DateOnly Date,
    TimeOnly? CheckInTime,
    TimeOnly? CheckOutTime,
    string? Notes) : IRequest<Guid>;
