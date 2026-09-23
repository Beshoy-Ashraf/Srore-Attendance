using Domain.Entities;
using Domain.Models;

namespace Domain.Interfaces;

public interface IAttendanceRepository : IBaseRepository<Attendance>
{
    Task<Attendance?> GetOpenAttendanceAsync(Guid staffId);

    Task<Attendance?> GetDetailedByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<PagedList<Attendance>> GetPagedAsync(AttendanceFilter filter, CancellationToken cancellationToken);

    /// <summary>Attendance whose check-in falls in [fromUtc, toUtcExclusive), oldest first.</summary>
    Task<IReadOnlyList<Attendance>> GetInRangeAsync(
        IReadOnlyCollection<Guid> staffIds, DateTime fromUtc, DateTime toUtcExclusive, CancellationToken cancellationToken);
}
