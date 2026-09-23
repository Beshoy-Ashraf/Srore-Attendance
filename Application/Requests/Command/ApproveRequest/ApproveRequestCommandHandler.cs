using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;

namespace Application.Requests.Commands.ApproveRequest;

public class ApproveRequestCommandHandler(IUnitOfWork unitOfWork, IAccessService access, IClock clock)
    : IRequestHandler<ApproveRequestCommand>
{
      public async Task Handle(ApproveRequestCommand request, CancellationToken cancellationToken)
      {
            await access.EnsureRoleAsync(cancellationToken, UserRole.Admin, UserRole.AreaManager);
            var me = await access.GetCurrentUserAsync(cancellationToken);

            var entity = await unitOfWork.RequestRepository.GetByIdAsync(request.Id, cancellationToken)
                ?? throw new NotFoundException(nameof(Request), request.Id);

            await access.EnsureStaffAccessAsync(entity.StaffId, cancellationToken);

            if (entity.Status != RequestStatus.Pending)
                  throw new BadRequestException("Only pending requests can be approved.");

            var now = clock.UtcNow;
            entity.Status = RequestStatus.Approved;
            entity.ApprovedByAreaManagerId = me.Id;
            entity.ApprovedDate = now;
            entity.UpdateDate = now;

            // Annual/sick leave map onto their matching shift type. Official holidays are a
            // company-wide non-working day (OFF), not personal annual leave.
            var shiftType = entity.Type switch
            {
                  RequestType.Annual => ShiftType.ANN,
                  RequestType.SickLeave => ShiftType.SL,
                  RequestType.OfficialHoliday => ShiftType.OFF,
                  _ => (ShiftType?)null
            };

            if (shiftType is not null)
            {
                  for (var date = entity.DateFrom; date <= entity.DateTo; date = date.AddDays(1))
                  {
                        // The staff member's live (non-soft-deleted) schedule for that day, if any.
                        var schedule = await unitOfWork.ScheduleRepository.GetByStaffAndDateAsync(entity.StaffId, date);

                        if (schedule is null)
                        {
                              schedule = new Schedule
                              {
                                    Id = Guid.NewGuid(),
                                    StaffId = entity.StaffId,
                                    Date = date,
                                    ShiftType = shiftType.Value,
                                    StartTime = TimeOnly.MinValue,
                                    EndTime = TimeOnly.MinValue,
                                    Status = ScheduleStatus.Approved,
                                    CreatedByStoreManagerId = me.Id,
                                    ApprovedByAreaManagerId = me.Id,
                                    ApprovedDate = now,
                                    CreatedDate = now
                              };
                              await unitOfWork.ScheduleRepository.AddAsync(schedule, cancellationToken);
                        }
                        else
                        {
                              schedule.ShiftType = shiftType.Value;
                              schedule.Status = ScheduleStatus.Approved;
                              schedule.ApprovedByAreaManagerId = me.Id;
                              schedule.ApprovedDate = now;
                              schedule.UpdateDate = now;
                        }
                  }
            }

            await unitOfWork.Complete(cancellationToken);
      }
}
