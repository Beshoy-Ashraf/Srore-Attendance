using Application.Common;
using FluentValidation;

namespace Application.Stores.Commands.CreateStore;

public class CreateStoreCommandValidator : AbstractValidator<CreateStoreCommand>
{
    public CreateStoreCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);

        RuleFor(x => x.DeviceMacs)
            .NotEmpty().WithMessage("At least one device MAC is required.");

        RuleForEach(x => x.DeviceMacs)
            .Must(mac => MacAddress.IsValid(mac))
            .WithMessage("Each device MAC must be a valid MAC address, e.g. 00:1A:2B:3C:4D:5E.");
    }
}
