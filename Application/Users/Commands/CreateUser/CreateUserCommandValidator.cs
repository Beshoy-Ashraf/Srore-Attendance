using FluentValidation;

namespace Application.Users.Commands.CreateUser;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
      public CreateUserCommandValidator()
      {
            RuleFor(x => x.Username).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(254);
            RuleFor(x => x.Password).NotEmpty().MinimumLength(8).MaximumLength(128);
            RuleFor(x => x.DisplayName).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Role).IsInEnum();
      }
}
