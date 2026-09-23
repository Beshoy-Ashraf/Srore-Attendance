using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class ScheduleRepository(AppDbContext context) : BaseRepository<Schedule>(context), IScheduleRepository
{
    private readonly AppDbContext _context = context;

    private static IQueryable<Schedule> WithDetails(IQueryable<Schedule> query) =>
        query
            .Include(s => s.Staff).ThenInclude(u => u.Store)
            .Include(s => s.CreatedByStoreManager)
            .Include(s => s.ApprovedByAreaManager);

    // The global filter (DeletedDate == null) makes "the live schedule of that day" the only match.
    public async Task<Schedule?> GetByStaffAndDateAsync(Guid staffId, DateOnly date) =>
        await _context.Set<Schedule>()
            .FirstOrDefaultAsync(s => s.StaffId == staffId && s.Date == date);

    public async Task<Schedule?> GetDetailedByIdAsync(Guid id, bool includeDeleted, CancellationToken cancellationToken)
    {
        IQueryable<Schedule> query = _context.Set<Schedule>();
        if (includeDeleted)
            query = query.IgnoreQueryFilters();

        return await WithDetails(query).FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Schedule>> GetByIdsAsync(IReadOnlyCollection<Guid> ids, CancellationToken cancellationToken)
    {
        var idList = ids.ToList();
        return await _context.Set<Schedule>()
            .Include(s => s.Staff)
            .Where(s => idList.Contains(s.Id))
            .ToListAsync(cancellationToken);
    }

    public async Task<PagedList<Schedule>> GetPagedAsync(ScheduleFilter filter, CancellationToken cancellationToken)
    {
        IQueryable<Schedule> query = _context.Set<Schedule>();

        // A rejected schedule is soft-deleted; asking for "Rejected" is how the history is read.
        if (filter.Status == ScheduleStatus.Rejected)
            query = query.IgnoreQueryFilters();

        query = WithDetails(query);

        if (filter.StaffId.HasValue)
            query = query.Where(s => s.StaffId == filter.StaffId.Value);

        if (filter.StoreIds is not null)
        {
            var storeIds = filter.StoreIds.ToList();
            query = query.Where(s => s.Staff.StoreId != null && storeIds.Contains(s.Staff.StoreId.Value));
        }

        if (filter.From.HasValue)
            query = query.Where(s => s.Date >= filter.From.Value);

        if (filter.To.HasValue)
            query = query.Where(s => s.Date <= filter.To.Value);

        if (filter.Status.HasValue)
            query = query.Where(s => s.Status == filter.Status.Value);

        var total = await query.CountAsync(cancellationToken);

        IQueryable<Schedule> ordered = filter.Status is ScheduleStatus.Approved or ScheduleStatus.Rejected
            ? query.OrderByDescending(s => s.ApprovedDate).ThenBy(s => s.Date).ThenBy(s => s.Id)
            : query.OrderBy(s => s.Date).ThenBy(s => s.StartTime).ThenBy(s => s.Id);

        var items = await ordered
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedList<Schedule>(items, total);
    }

    public async Task<IReadOnlyList<Schedule>> GetApprovedInRangeAsync(
        IReadOnlyCollection<Guid> staffIds, DateOnly from, DateOnly to, CancellationToken cancellationToken)
    {
        var ids = staffIds.ToList();
        return await _context.Set<Schedule>()
            .Where(s => ids.Contains(s.StaffId)
                        && s.Status == ScheduleStatus.Approved
                        && s.Date >= from && s.Date <= to)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<PendingScheduleSlice>> GetPendingSlicesAsync(
        IReadOnlyCollection<Guid>? storeIds, CancellationToken cancellationToken)
    {
        IQueryable<Schedule> query = _context.Set<Schedule>()
            .Where(s => s.Status == ScheduleStatus.Pending && s.Staff.StoreId != null && s.Staff.Store != null);

        if (storeIds is not null)
        {
            var ids = storeIds.ToList();
            query = query.Where(s => s.Staff.StoreId != null && ids.Contains(s.Staff.StoreId.Value));
        }

        var rows = await query
            .Select(s => new { StoreId = s.Staff.StoreId!.Value, StoreName = s.Staff.Store!.Name, s.Date })
            .ToListAsync(cancellationToken);

        return rows.Select(r => new PendingScheduleSlice(r.StoreId, r.StoreName, r.Date)).ToList();
    }
}
