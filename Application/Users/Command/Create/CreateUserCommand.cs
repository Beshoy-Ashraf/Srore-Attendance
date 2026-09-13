using Domain.Enums;
using MediatR;

namespace Application.Users.Commands.CreateUser;

public record CreateUserCommand(
    string Username,
    string Email,
    string Password,
    string DisplayName,
    UserRole Role,
    Guid? StoreId,
    string? ProfileImageUrl) : IRequest<Guid>;