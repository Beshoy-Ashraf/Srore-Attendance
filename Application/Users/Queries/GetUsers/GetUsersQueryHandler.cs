using Application.Users.Dtos;
using Domain.Interfaces;
using MediatR;

namespace Application.Users.Queries.GetUsers;

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, IEnumerable<UserDto>>
{
      private readonly IUnitOfWork _unitOfWork;

      public GetUsersQueryHandler(IUnitOfWork unitOfWork)
      {
            _unitOfWork = unitOfWork;
      }

      public async Task<IEnumerable<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
      {
            var users = await _unitOfWork.UserRepository.GetFilteredAsync(
                request.StoreId, request.Role, request.Page, request.PageSize, cancellationToken);

            if (users == null || !users.Any())
                  return [];


            return users.Where(u => u.DeleteDate == null)
                .Select(u => new UserDto(
                    u.Id,
                    u.Username,
                    u.Email,
                    u.ProfilePictureUrl,
                    u.DisplayName,
                    u.Role,
                    u.StoreId));
      }
}