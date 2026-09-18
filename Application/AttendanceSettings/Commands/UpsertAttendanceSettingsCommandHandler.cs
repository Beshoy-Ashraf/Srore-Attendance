using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;

namespace Application.AttendanceSettings.Commands.UpsertAttendanceSettings;

public class UpsertAttendanceSettingsCommandHandler : IRequestHandler<UpsertAttendanceSettingsCommand, Guid>
{
      private readonly IUnitOfWork _unitOfWork;

      public UpsertAttendanceSettingsCommandHandler(IUnitOfWork unitOfWork)
      {
            _unitOfWork = unitOfWork;
      }

      public async Task<Guid> Handle(UpsertAttendanceSettingsCommand request, CancellationToken cancellationToken)
      {
            var store = await _unitOfWork.StoreRepository.GetByIdAsync(request.StoreId, cancellationToken)
                ?? throw new NotFoundException(nameof(Store), request.StoreId);

            var settings = await _unitOfWork.AttendanceSettingsRepository.GetByStoreIdAsync(request.StoreId);

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
                        CreatedDate = DateTime.UtcNow
                  };
                  await _unitOfWork.AttendanceSettingsRepository.AddAsync(settings, cancellationToken);
            }
            else
            {
                  settings.MorningStart = request.MorningStart;
                  settings.MorningEnd = request.MorningEnd;
                  settings.NightStart = request.NightStart;
                  settings.NightEnd = request.NightEnd;
                  settings.LateGraceMinutes = request.LateGraceMinutes;
                  settings.UpdateDate = DateTime.UtcNow;

                  await _unitOfWork.AttendanceSettingsRepository.UpdateAsync(settings, cancellationToken);
            }

            await _unitOfWork.Complete(cancellationToken);

            return settings.Id;
      }
}