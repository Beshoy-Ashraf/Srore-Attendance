using Application.Common.Interfaces;
using Application.Users.Dtos;
using Application.Users.Mappings;
using Domain.Interfaces;
using MediatR;

namespace Application.Users.Queries.GetUserById;

public class GetUserByIdQueryHandler(IAccessService access) : IRequestHandler<GetUserByIdQuery, UserDto>
{
      public async Task<UserDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
      {
            var user = await access.EnsureUserVisibleAsync(request.Id, cancellationToken);
            return user.ToDto();
      }
}
