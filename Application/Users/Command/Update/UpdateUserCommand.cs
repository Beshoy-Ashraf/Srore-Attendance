using Domain.Enums;
using MediatR;

namespace Application.Users.Commands.UpdateUser;

public record UpdateUserCommand(
    Guid Id,
    string DisplayName,
    string? ProfilePictureUrl,
    UserRole Role,
    Guid? StoreId) : IRequest;