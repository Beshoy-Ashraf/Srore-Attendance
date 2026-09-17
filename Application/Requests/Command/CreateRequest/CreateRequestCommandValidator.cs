using Domain.Enums;
using FluentValidation;

namespace Application.Requests.Commands.CreateRequest;

public class CreateRequestCommandValidator : AbstractValidator<CreateRequestCommand>
{
      public CreateRequestCommandValidator()
      {
            RuleFor(x => x.StaffId).NotEmpty();
            RuleFor(x => x.RequestedById).NotEmpty();
            RuleFor(x => x.Type).IsInEnum();
            RuleFor(x => x.Reason).NotEmpty().MaximumLength(500);
            RuleFor(x => x.DateTo).GreaterThanOrEqualTo(x => x.DateFrom)
                .WithMessage("DateTo must be on or after DateFrom.");
      }
}