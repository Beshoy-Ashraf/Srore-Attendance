using Application.Attendance.Dtos;
using MediatR;

namespace Application.Attendance.Queries.GetAttendances;

public record GetAttendancesQuery(
    Guid? StaffId,
    Guid? StoreId,
    DateOnly? From,
    DateOnly? To,
    int Page = 1,
    int PageSize = 20) : IRequest<IEnumerable<AttendanceDto>>;