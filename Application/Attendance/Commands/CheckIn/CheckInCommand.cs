using MediatR;

namespace Application.Attendance.Commands.CheckIn;

public record CheckInCommand(
    Guid StaffId,
    string RouterMac,
    string DeviceMac) : IRequest<CheckInResponseDto>;