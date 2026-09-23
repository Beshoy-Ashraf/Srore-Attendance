using FluentValidation;

namespace Application.Missions.Commands.RejectMission;

public class RejectMissionCommandValidator : AbstractValidator<RejectMissionCommand>
{
      public RejectMissionCommandValidator()
      {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Reason).NotEmpty().MaximumLength(500);
      }
}
