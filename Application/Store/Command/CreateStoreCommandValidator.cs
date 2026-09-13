using FluentValidation;

namespace Application.Stores.Commands.CreateStore;

public class CreateStoreCommandValidator : AbstractValidator<CreateStoreCommand>
{
      private const string MacPattern = "^([0-9A-Fa-f]{2}:){5}[0-9A-Fa-f]{2}$";

      public CreateStoreCommandValidator()
      {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(150);

            RuleFor(x => x.RouterMacs)
                .NotEmpty().WithMessage("At least one router MAC is required.");

            RuleForEach(x => x.RouterMacs)
                .Matches(MacPattern)
                .WithMessage("Each router MAC must be a valid MAC address, e.g. 00:1A:2B:3C:4D:5E.");
      }
}