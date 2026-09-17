using FluentValidation;

namespace Application.Users.Commands.UpdateUser;

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
      public UpdateUserCommandValidator()
      {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.DisplayName).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Role).IsInEnum();
      }
}