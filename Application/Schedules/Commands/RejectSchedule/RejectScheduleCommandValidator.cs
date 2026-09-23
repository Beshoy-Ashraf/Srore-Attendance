using FluentValidation;

namespace Application.Schedules.Commands.RejectSchedule;

public class RejectScheduleCommandValidator : AbstractValidator<RejectScheduleCommand>
{
      public RejectScheduleCommandValidator()
      {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Reason).NotEmpty().MaximumLength(500);
      }
}
