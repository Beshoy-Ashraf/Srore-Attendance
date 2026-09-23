using MediatR;

namespace Application.Schedules.Commands.ApproveSchedule;

public record ApproveScheduleCommand(Guid Id) : IRequest;
