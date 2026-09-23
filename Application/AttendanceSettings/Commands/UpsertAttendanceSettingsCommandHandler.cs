using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;

namespace Application.AttendanceSettings.Commands.UpsertAttendanceSettings;

public class UpsertAttendanceSettingsCommandHandler(IUnitOfWork unitOfWork, IAccessService access, IClock clock)
    : IRequestHandler<UpsertAttendanceSettingsCommand, Guid>
{
      public async Task<Guid> Handle(UpsertAttendanceSettingsCommand request, CancellationToken cancellationToken)
      {
            await access.EnsureStoreAccessAsync(request.StoreId, cancellationToken);

            var store = await unitOfWork.StoreRepository.GetByIdAsync(request.StoreId, cancellationToken)
                ?? throw new NotFoundException(nameof(Store), request.StoreId);

            var settings = await unitOfWork.AttendanceSettingsRepository.GetByStoreIdAsync(request.StoreId);
            var now = clock.UtcNow;

            if (settings is null)
            {
                  settings = new Domain.Entities.AttendanceSettings
                  {
                        Id = Guid.NewGuid(),
                        StoreId = request.StoreId,
                        MorningStart = request.MorningStart,
                        MorningEnd = request.MorningEnd,
                        NightStart = request.NightStart,
                        NightEnd = request.NightEnd,
                        LateGraceMinutes = request.LateGraceMinutes,
                        CreatedDate = now
                  };
                  await unitOfWork.AttendanceSettingsRepository.AddAsync(settings, cancellationToken);
            }
            else
            {
                  settings.MorningStart = request.MorningStart;
                  settings.MorningEnd = request.MorningEnd;
                  settings.NightStart = request.NightStart;
                  settings.NightEnd = request.NightEnd;
                  settings.LateGraceMinutes = request.LateGraceMinutes;
                  settings.UpdateDate = now;
            }

            await unitOfWork.Complete(cancellationToken);

            return settings.Id;
      }
}
