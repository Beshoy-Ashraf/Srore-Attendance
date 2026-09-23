using FluentValidation;

namespace Application.Users.Commands.ResetPassword;

public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
      public ResetPasswordCommandValidator()
      {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.NewPassword).NotEmpty().MinimumLength(8).MaximumLength(128);
      }
}
