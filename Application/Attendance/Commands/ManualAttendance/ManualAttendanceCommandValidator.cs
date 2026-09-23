using FluentValidation;

namespace Application.Attendance.Commands.ManualAttendance;

public class ManualAttendanceCommandValidator : AbstractValidator<ManualAttendanceCommand>
{
      public ManualAttendanceCommandValidator()
      {
            RuleFor(x => x.StaffId).NotEmpty();

            RuleFor(x => x)
                .Must(x => x.CheckInTime is not null || x.CheckOutTime is not null)
                .WithMessage("At least one of CheckInTime or CheckOutTime must be provided.");

            RuleFor(x => x)
                .Must(x => x.CheckInTime is null || x.CheckOutTime is null || x.CheckOutTime > x.CheckInTime)
                .WithMessage("CheckOutTime must be after CheckInTime.");
      }
}