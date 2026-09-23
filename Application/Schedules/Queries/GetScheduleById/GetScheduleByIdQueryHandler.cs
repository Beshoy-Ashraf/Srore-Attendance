using Application.Common.Interfaces;
using Application.Schedules.Dtos;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;

namespace Application.Schedules.Queries.GetScheduleById;

public class GetScheduleByIdQueryHandler(IUnitOfWork unitOfWork, IAccessService access)
    : IRequestHandler<GetScheduleByIdQuery, ScheduleDto>
{
      public async Task<ScheduleDto> Handle(GetScheduleByIdQuery request, CancellationToken cancellationToken)
      {
            // Look up including soft-deleted (rejected) rows: only the access check below decides visibility.
            var schedule = await unitOfWork.ScheduleRepository.GetDetailedByIdAsync(request.Id, includeDeleted: true, cancellationToken)
                ?? throw new NotFoundException(nameof(Schedule), request.Id);

            await access.EnsureStaffAccessAsync(schedule.StaffId, cancellationToken);

            return schedule.ToDto();
      }
}
