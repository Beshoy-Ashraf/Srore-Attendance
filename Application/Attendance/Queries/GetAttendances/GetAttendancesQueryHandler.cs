using Application.Attendance.Dtos;
using Domain.Interfaces;
using MediatR;

namespace Application.Attendance.Queries.GetAttendances;

public class GetAttendancesQueryHandler : IRequestHandler<GetAttendancesQuery, IEnumerable<AttendanceDto>>
{
      private readonly IUnitOfWork _unitOfWork;

      public GetAttendancesQueryHandler(IUnitOfWork unitOfWork)
      {
            _unitOfWork = unitOfWork;
      }

      public async Task<IEnumerable<AttendanceDto>> Handle(GetAttendancesQuery request, CancellationToken cancellationToken)
      {
            var attendances = await _unitOfWork.AttendanceRepository.GetFilteredAsync(
                request.StaffId, request.StoreId, request.From, request.To, request.Page, request.PageSize);

            return attendances.Select(a => new AttendanceDto(
                a.Id,
                a.StaffId,
                a.ScheduleId,
                a.CheckInTime,
                a.CheckOutTime,
                a.CheckInRouterMac,
                a.CheckInDeviceMac,
                a.CheckInIp,
                a.VerificationMethod,
                a.IsLate,
                a.EnteredManuallyBy,
                a.Notes));
      }
}