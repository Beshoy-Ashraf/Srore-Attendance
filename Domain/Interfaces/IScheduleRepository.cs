using Domain.Entities;
using Domain.Models;

namespace Domain.Interfaces;

public interface IScheduleRepository : IBaseRepository<Schedule>
{
    /// <summary>The live (not soft-deleted) schedule of a staff member on a date, whatever its status.</summary>
    Task<Schedule?> GetByStaffAndDateAsync(Guid staffId, DateOnly date);

    /// <summary>Loads a schedule with staff, store, creator and approver. Rejected schedules are soft-deleted,
    /// so <paramref name="includeDeleted"/> is needed to read them back.</summary>
    Task<Schedule?> GetDetailedByIdAsync(Guid id, bool includeDeleted, CancellationToken cancellationToken);

    Task<IReadOnlyList<Schedule>> GetByIdsAsync(IReadOnlyCollection<Guid> ids, CancellationToken cancellationToken);

    /// <summary>Filtered page. When the filter asks for <c>Rejected</c> the soft-deleted rows are included
    /// (that is the only way to see them); every other status excludes soft-deleted rows.</summary>
    Task<PagedList<Schedule>> GetPagedAsync(ScheduleFilter filter, CancellationToken cancellationToken);

    Task<IReadOnlyList<Schedule>> GetApprovedInRangeAsync(
        IReadOnlyCollection<Guid> staffIds, DateOnly from, DateOnly to, CancellationToken cancellationToken);

    Task<IReadOnlyList<PendingScheduleSlice>> GetPendingSlicesAsync(
        IReadOnlyCollection<Guid>? storeIds, CancellationToken cancellationToken);
}
