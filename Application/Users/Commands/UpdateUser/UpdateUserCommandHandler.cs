using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;

namespace Application.Users.Commands.UpdateUser;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand>
{
      private readonly IUnitOfWork _unitOfWork;

      public UpdateUserCommandHandler(IUnitOfWork unitOfWork)
      {
            _unitOfWork = unitOfWork;
      }

      public async Task Handle(UpdateUserCommand request, CancellationToken cancellationToken)
      {
            var user = await _unitOfWork.UserRepository.GetUserByUserId(request.Id, cancellationToken)
                ?? throw new NotFoundException(nameof(User), request.Id);
            
            user.DisplayName = request.DisplayName;
            user.ProfilePictureUrl = request.ProfilePictureUrl?.ToString() ?? string.Empty;
            user.Role = request.Role;
            user.StoreId = request.StoreId;
            user.UpdateDate = DateTime.UtcNow;

            await _unitOfWork.UserRepository.UpdateAsync(user, cancellationToken);
            await _unitOfWork.Complete(cancellationToken);
      }
}