using FluentValidation;

namespace Application.Requests.Commands.RejectRequest;

public class RejectRequestCommandValidator : AbstractValidator<RejectRequestCommand>
{
      public RejectRequestCommandValidator()
      {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Reason).NotEmpty().MaximumLength(500);
      }
}
