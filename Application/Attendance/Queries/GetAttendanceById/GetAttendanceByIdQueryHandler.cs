using Application.Attendance.Dtos;
using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;

namespace Application.Attendance.Queries.GetAttendanceById;

public class GetAttendanceByIdQueryHandler : IRequestHandler<GetAttendanceByIdQuery, AttendanceDto>
{
      private readonly IUnitOfWork _unitOfWork;

      public GetAttendanceByIdQueryHandler(IUnitOfWork unitOfWork)
      {
            _unitOfWork = unitOfWork;
      }

      public async Task<AttendanceDto> Handle(GetAttendanceByIdQuery request, CancellationToken cancellationToken)
      {
            var attendance = await _unitOfWork.AttendanceRepository.GetByIdAsync(request.Id, cancellationToken)
                ?? throw new NotFoundException(nameof(Domain.Entities.Attendance), request.Id);

            return new AttendanceDto(
                attendance.Id,
                attendance.StaffId,
                attendance.ScheduleId,
                attendance.CheckInTime,
                attendance.CheckOutTime,
                attendance.CheckInRouterMac,
                attendance.CheckInDeviceMac,
                attendance.CheckInIp,
                attendance.VerificationMethod,
                attendance.IsLate,
                attendance.EnteredManuallyBy,
                attendance.Notes);
      }
}