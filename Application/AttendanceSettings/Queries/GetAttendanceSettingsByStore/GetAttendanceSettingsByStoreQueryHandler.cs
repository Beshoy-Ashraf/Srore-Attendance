using Application.AttendanceSettings.Dtos;
using Application.Common.Interfaces;
using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;

namespace Application.AttendanceSettings.Queries.GetAttendanceSettingsByStore;

public class GetAttendanceSettingsByStoreQueryHandler(IUnitOfWork unitOfWork, IAccessService access)
    : IRequestHandler<GetAttendanceSettingsByStoreQuery, AttendanceSettingsDto>
{
      public async Task<AttendanceSettingsDto> Handle(
          GetAttendanceSettingsByStoreQuery request, CancellationToken cancellationToken)
      {
            await access.EnsureStoreAccessAsync(request.StoreId, cancellationToken);

            var settings = await unitOfWork.AttendanceSettingsRepository.GetByStoreIdAsync(request.StoreId)
                ?? throw new NotFoundException(nameof(Domain.Entities.AttendanceSettings), request.StoreId);

            return new AttendanceSettingsDto(
                settings.Id,
                settings.StoreId,
                settings.MorningStart,
                settings.MorningEnd,
                settings.NightStart,
                settings.NightEnd,
                settings.LateGraceMinutes);
      }
}
