using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;

namespace Application.Requests.Commands.ApproveRequest;

public class ApproveRequestCommandHandler : IRequestHandler<ApproveRequestCommand>
{
      private readonly IUnitOfWork _unitOfWork;

      public ApproveRequestCommandHandler(IUnitOfWork unitOfWork)
      {
            _unitOfWork = unitOfWork;
      }

      public async Task Handle(ApproveRequestCommand request, CancellationToken cancellationToken)
      {
            var entity = await _unitOfWork.RequestRepository.GetByIdAsync(request.Id, cancellationToken)
                ?? throw new NotFoundException(nameof(Request), request.Id);

            if (entity.Status != RequestStatus.Pending)
                  throw new BadRequestException("Only pending requests can be approved.");

            entity.Status = RequestStatus.Approved;
            entity.ApprovedByAreaManagerId = request.ApprovedByAreaManagerId;
            entity.ApprovedDate = DateTime.UtcNow;
            entity.UpdateDate = DateTime.UtcNow;

            await _unitOfWork.RequestRepository.UpdateAsync(entity, cancellationToken);

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
                        var schedule = await _unitOfWork.ScheduleRepository.GetByStaffAndDateAsync(entity.StaffId, date);

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
                                    CreatedByStoreManagerId = request.ApprovedByAreaManagerId,
                                    ApprovedByAreaManagerId = request.ApprovedByAreaManagerId,
                                    ApprovedDate = DateTime.UtcNow,
                                    CreatedDate = DateTime.UtcNow
                              };
                              await _unitOfWork.ScheduleRepository.AddAsync(schedule, cancellationToken);
                        }
                        else
                        {
                              schedule.ShiftType = shiftType.Value;
                              schedule.Status = ScheduleStatus.Approved;
                              schedule.ApprovedByAreaManagerId = request.ApprovedByAreaManagerId;
                              schedule.ApprovedDate = DateTime.UtcNow;
                              schedule.UpdateDate = DateTime.UtcNow;
                              await _unitOfWork.ScheduleRepository.UpdateAsync(schedule, cancellationToken);
                        }
                  }
            }

            await _unitOfWork.Complete(cancellationToken);
      }
}