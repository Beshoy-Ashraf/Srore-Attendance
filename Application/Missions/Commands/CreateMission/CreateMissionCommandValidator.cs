using FluentValidation;

namespace Application.Missions.Commands.CreateMission;

public class CreateMissionCommandValidator : AbstractValidator<CreateMissionCommand>
{
      public CreateMissionCommandValidator()
      {
            RuleFor(x => x.StaffId).NotEmpty();
            RuleFor(x => x.Reason).NotEmpty().MaximumLength(500);
            RuleFor(x => x.DateTo).GreaterThanOrEqualTo(x => x.DateFrom)
                .WithMessage("DateTo must be on or after DateFrom.");
      }
}