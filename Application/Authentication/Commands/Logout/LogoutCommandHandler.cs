using MediatR;
using Domain.Interfaces;

namespace Application.Authentication.Commands.Logout;

public class LogoutCommandHandler(IUnitOfWork context) : IRequestHandler<LogoutCommand>
{
      public async Task Handle(LogoutCommand request, CancellationToken cancellationToken)
      {
            var user = await context.UserRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (user is null)
                  return;

            var token = user.RefreshTokens.FirstOrDefault(rt => rt.Token == request.RefreshToken);
            if (token is not null && !token.IsRevoked)
            {
                  token.IsRevoked = true;
                  await context.Complete(cancellationToken);
            }
      }
}
