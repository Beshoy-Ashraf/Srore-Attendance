using Application.Attendance.Dtos;
using MediatR;

namespace Application.Attendance.Queries.GetAttendanceById;

public record GetAttendanceByIdQuery(Guid Id) : IRequest<AttendanceDto>;