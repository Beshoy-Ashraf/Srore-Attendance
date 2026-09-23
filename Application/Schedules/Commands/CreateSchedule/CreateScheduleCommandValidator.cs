using Domain.Enums;
using FluentValidation;

namespace Application.Schedules.Commands.CreateSchedule;

public class CreateScheduleCommandValidator : AbstractValidator<CreateScheduleCommand>
{
      public CreateScheduleCommandValidator()
      {
            RuleFor(x => x.StaffId).NotEmpty();
            RuleFor(x => x.ShiftType).IsInEnum();

            // ANN (annual leave), SL (seek leave), and OFF (day off) are non-working types — no meaningful working hours
            RuleFor(x => x)
                .Must(x => x.ShiftType is ShiftType.ANN or ShiftType.SL or ShiftType.OFF || x.EndTime > x.StartTime)
                .WithMessage("EndTime must be after StartTime for working shifts.");
      }
}