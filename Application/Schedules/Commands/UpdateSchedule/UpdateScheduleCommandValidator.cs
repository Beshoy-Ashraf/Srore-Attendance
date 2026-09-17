using Domain.Enums;
using FluentValidation;

namespace Application.Schedules.Commands.UpdateSchedule;

public class UpdateScheduleCommandValidator : AbstractValidator<UpdateScheduleCommand>
{
      public UpdateScheduleCommandValidator()
      {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.ShiftType).IsInEnum();

            RuleFor(x => x)
                .Must(x => x.ShiftType is ShiftType.ANN or ShiftType.SL || x.EndTime > x.StartTime)
                .WithMessage("EndTime must be after StartTime for working shifts.");
      }
}