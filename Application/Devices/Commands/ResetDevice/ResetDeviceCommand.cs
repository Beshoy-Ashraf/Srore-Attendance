using MediatR;

namespace Application.Devices.Commands.ResetDevice;

/// <summary>Clears a staff member's registered device MAC (e.g. they got a new PC) so the next
/// check-in registers whatever machine they use. Optionally deactivates instead of clearing immediately.</summary>
public record ResetDeviceCommand(Guid StaffId) : IRequest;
