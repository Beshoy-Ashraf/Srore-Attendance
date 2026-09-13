using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;

namespace Application.Users.Commands.CreateUser;

public class CreateUserCommandHandler(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher) : IRequestHandler<CreateUserCommand, Guid>
{
      private readonly IUnitOfWork _unitOfWork = unitOfWork;
      private readonly IPasswordHasher _passwordHasher = passwordHasher;

      public async Task<Guid> Handle(CreateUserCommand request, CancellationToken cancellationToken)
      {
            var existing = await _unitOfWork.UserRepository.GetUserByEmail(request.Email, cancellationToken);
            var existingUsername = await _unitOfWork.UserRepository.GetUserByUsername(request.Username, cancellationToken);
            if (existing is not null)
                  throw new ConflictException($"A user with email '{request.Email}' already exists.");

            if (existingUsername is not null)
                  throw new ConflictException($"A user with username '{request.Username}' already exists.");

            var user = new User(
                  request.Username,
                  request.Email,
                  _passwordHasher.Hash(request.Password),
                  request.DisplayName,
                  request.ProfileImageUrl?.ToString() ?? string.Empty,
                  request.Role
            )
            {
                  StoreId = request.StoreId

            };


            await _unitOfWork.UserRepository.AddAsync(user, cancellationToken);
            await _unitOfWork.Complete(cancellationToken);

            return user.Id;
      }
}