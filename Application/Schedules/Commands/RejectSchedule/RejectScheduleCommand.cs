using MediatR;

namespace Application.Schedules.Commands.RejectSchedule;

public record RejectScheduleCommand(Guid Id, string Reason) : IRequest;
