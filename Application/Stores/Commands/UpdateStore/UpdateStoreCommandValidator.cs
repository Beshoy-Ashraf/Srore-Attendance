using Application.Common;
using FluentValidation;

namespace Application.Stores.Commands.UpdateStore;

public class UpdateStoreCommandValidator : AbstractValidator<UpdateStoreCommand>
{
    public UpdateStoreCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);

        RuleFor(x => x.DevicesMacs)
            .NotEmpty().WithMessage("At least one device MAC is required.");

        RuleForEach(x => x.DevicesMacs)
            .Must(mac => MacAddress.IsValid(mac))
            .WithMessage("Each device MAC must be a valid MAC address, e.g. 00:1A:2B:3C:4D:5E.");
    }
}
