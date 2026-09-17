using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;

namespace Application.Users.Commands.DeleteUser;

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand>
{
      private readonly IUnitOfWork _unitOfWork;

      public DeleteUserCommandHandler(IUnitOfWork unitOfWork)
      {
            _unitOfWork = unitOfWork;
      }

      public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
      {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(request.Id, cancellationToken)
                ?? throw new NotFoundException(nameof(User), request.Id);

            user.DeleteDate = DateTime.UtcNow;

            await _unitOfWork.UserRepository.UpdateAsync(user, cancellationToken);
            await _unitOfWork.Complete(cancellationToken);
      }
}