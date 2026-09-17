using Application.Users.Dtos;
using MediatR;

namespace Application.Users.Queries.GetUserById;

public record GetUserByIdQuery(Guid Id) : IRequest<UserDto>;