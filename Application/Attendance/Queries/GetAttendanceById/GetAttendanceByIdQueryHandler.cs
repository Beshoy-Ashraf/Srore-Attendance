using Application.Attendance.Dtos;
using Application.Common.Interfaces;
using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;

namespace Application.Attendance.Queries.GetAttendanceById;

public class GetAttendanceByIdQueryHandler(IUnitOfWork unitOfWork, IAccessService access)
    : IRequestHandler<GetAttendanceByIdQuery, AttendanceDto>
{
      public async Task<AttendanceDto> Handle(GetAttendanceByIdQuery request, CancellationToken cancellationToken)
      {
            var attendance = await unitOfWork.AttendanceRepository.GetDetailedByIdAsync(request.Id, cancellationToken)
                ?? throw new NotFoundException(nameof(Domain.Entities.Attendance), request.Id);

            await access.EnsureStaffAccessAsync(attendance.StaffId, cancellationToken);

            return attendance.ToDto();
      }
}
