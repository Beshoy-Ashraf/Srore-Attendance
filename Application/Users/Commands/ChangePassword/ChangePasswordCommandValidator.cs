using FluentValidation;

namespace Application.Users.Commands.ChangePassword;

public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
      public ChangePasswordCommandValidator()
      {
            RuleFor(x => x.CurrentPassword).NotEmpty();
            RuleFor(x => x.NewPassword).NotEmpty().MinimumLength(8).MaximumLength(128);
            RuleFor(x => x).Must(x => x.NewPassword != x.CurrentPassword)
                  .WithMessage("The new password must be different from the current one.");
      }
}
