using Application.Users.Dtos;
using Domain.Enums;
using MediatR;

namespace Application.Users.Queries.GetUsers;

public record GetUsersQuery(
    Guid? StoreId,
    UserRole? Role,
    int Page = 1,
    int PageSize = 20) : IRequest<IEnumerable<UserDto>>;