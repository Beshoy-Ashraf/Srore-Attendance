using Application.AttendanceSettings.Dtos;
using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;

namespace Application.AttendanceSettings.Queries.GetAttendanceSettingsByStore;

public class GetAttendanceSettingsByStoreQueryHandler
    : IRequestHandler<GetAttendanceSettingsByStoreQuery, AttendanceSettingsDto>
{
      private readonly IUnitOfWork _unitOfWork;

      public GetAttendanceSettingsByStoreQueryHandler(IUnitOfWork unitOfWork)
      {
            _unitOfWork = unitOfWork;
      }

      public async Task<AttendanceSettingsDto> Handle(
          GetAttendanceSettingsByStoreQuery request, CancellationToken cancellationToken)
      {
            var settings = await _unitOfWork.AttendanceSettingsRepository.GetByStoreIdAsync(request.StoreId)
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