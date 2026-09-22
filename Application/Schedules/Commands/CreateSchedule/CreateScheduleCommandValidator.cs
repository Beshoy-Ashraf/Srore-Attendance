using Domain.Enums;
using FluentValidation;

namespace Application.Schedules.Commands.CreateSchedule;

public class CreateScheduleCommandValidator : AbstractValidator<CreateScheduleCommand>
{
      public CreateScheduleCommandValidator()
      {
            RuleFor(x => x.StaffId).NotEmpty();
            RuleFor(x => x.ShiftType).IsInEnum();

            RuleFor(x => x)
                .Must(x => x.ShiftType is ShiftType.ANN or ShiftType.SL or ShiftType.OFF || x.EndTime > x.StartTime)
                .WithMessage("EndTime must be after StartTime for working shifts.");
      }
}