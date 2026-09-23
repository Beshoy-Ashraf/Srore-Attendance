using Application.Users.Dtos;
using Domain.Entities;

namespace Application.Users.Mappings;

public static class UserMappings
{
    public static UserDto ToDto(this User user) =>
        new(user.Id, user.Username, user.Email, user.ProfilePictureUrl, user.DisplayName, user.Role, user.StoreId);
}
