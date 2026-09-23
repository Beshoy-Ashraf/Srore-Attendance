using MediatR;

namespace Application.Users.Commands.ChangePassword;

/// <summary>The signed-in user changes their own password.</summary>
public record ChangePasswordCommand(string CurrentPassword, string NewPassword) : IRequest;
