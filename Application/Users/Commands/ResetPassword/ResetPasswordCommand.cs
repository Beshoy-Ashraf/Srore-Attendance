using MediatR;

namespace Application.Users.Commands.ResetPassword;

/// <summary>An admin (or the area manager of the user's store) sets a new password for someone else.</summary>
public record ResetPasswordCommand(Guid UserId, string NewPassword) : IRequest;
