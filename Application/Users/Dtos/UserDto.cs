using Domain.Enums;

namespace Application.Users.Dtos;

public record UserDto(Guid Id, string Username, string Email, string ProfilePictureUrl, string DisplayName, UserRole Role, Guid? StoreId);