using Application.Attendance.Dtos;
using Application.Common.Models;
using MediatR;

namespace Application.Attendance.Queries.GetAttendances;

public record GetAttendancesQuery(
    Guid? StaffId,
    Guid? StoreId,
    DateTime? From,
    DateTime? To,
    bool? LateOnly = null,
    int Page = 1,
    int PageSize = 20) : IRequest<PagedResult<AttendanceDto>>;
