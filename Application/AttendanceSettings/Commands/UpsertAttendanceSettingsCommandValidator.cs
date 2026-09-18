using FluentValidation;

namespace Application.AttendanceSettings.Commands.UpsertAttendanceSettings;

public class UpsertAttendanceSettingsCommandValidator : AbstractValidator<UpsertAttendanceSettingsCommand>
{
      public UpsertAttendanceSettingsCommandValidator()
      {
            RuleFor(x => x.StoreId).NotEmpty();
            RuleFor(x => x.LateGraceMinutes).InclusiveBetween(0, 120);

            RuleFor(x => x)
                .Must(x => x.MorningEnd > x.MorningStart)
                .WithMessage("MorningEnd must be after MorningStart.");

            RuleFor(x => x)
                .Must(x => x.NightEnd > x.NightStart)
                .WithMessage("NightEnd must be after NightStart.");
      }
}