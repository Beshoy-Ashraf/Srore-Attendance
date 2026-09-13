using Application.Users.Dtos;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;

namespace Application.Users.Queries.GetUserById;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDto>
{
      private readonly IUnitOfWork _unitOfWork;

      public GetUserByIdQueryHandler(IUnitOfWork unitOfWork)
      {
            _unitOfWork = unitOfWork;
      }

      public async Task<UserDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
      {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(request.Id, cancellationToken)
                ?? throw new NotFoundException(nameof(User), request.Id);

            return new UserDto(
                user.Id,
                user.Username,
                user.Email,
                user.DisplayName,
                user.ProfilePictureUrl,
                user.Role,
                user.StoreId);
      }
}