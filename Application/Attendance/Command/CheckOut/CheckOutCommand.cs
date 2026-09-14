using MediatR;

namespace Application.Attendance.Commands.CheckOut;

public record CheckOutCommand(
    Guid StaffId,
    string RouterMac,
    string DeviceMac) : IRequest<CheckOutResponseDto>;