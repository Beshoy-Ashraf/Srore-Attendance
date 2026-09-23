using MediatR;

namespace Application.Authentication.Commands.Logout;

/// <summary>Revokes one refresh token (this device/session). UserId comes from the caller's own access token.</summary>
public record LogoutCommand(Guid UserId, string RefreshToken) : IRequest;
