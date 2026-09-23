using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class MissionRepository(AppDbContext context) : BaseRepository<Mission>(context), IMissionRepository
{
    private readonly AppDbContext _context = context;

    private static IQueryable<Mission> WithDetails(IQueryable<Mission> query) =>
        query
            .Include(m => m.Staff).ThenInclude(u => u.Store)
            .Include(m => m.ApprovedByAreaManager);

    public async Task<Mission?> GetDetailedByIdAsync(Guid id, CancellationToken cancellationToken) =>
        await GetDetailedByIdAsync(id, includeDeleted: false, cancellationToken);

    public async Task<Mission?> GetDetailedByIdAsync(Guid id, bool includeDeleted, CancellationToken cancellationToken)
    {
        IQueryable<Mission> query = _context.Set<Mission>();
        if (includeDeleted)
            query = query.IgnoreQueryFilters();

        return await WithDetails(query).FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
    }

    public async Task<PagedList<Mission>> GetPagedAsync(MissionFilter filter, CancellationToken cancellationToken)
    {
        // A rejected mission is soft-deleted; asking for "Rejected" is how the history is read.
        IQueryable<Mission> query = filter.Status == RequestStatus.Rejected
            ? _context.Set<Mission>().IgnoreQueryFilters()
            : _context.Set<Mission>();

        query = WithDetails(query);

        if (filter.StaffId.HasValue)
            query = query.Where(m => m.StaffId == filter.StaffId.Value);

        if (filter.StoreIds is not null)
        {
            var storeIds = filter.StoreIds.ToList();
            query = query.Where(m => m.Staff.StoreId != null && storeIds.Contains(m.Staff.StoreId.Value));
        }

        if (filter.Status.HasValue)
            query = query.Where(m => m.Status == filter.Status.Value);

        if (filter.From.HasValue)
            query = query.Where(m => m.DateTo >= filter.From.Value);

        if (filter.To.HasValue)
            query = query.Where(m => m.DateFrom <= filter.To.Value);

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(m => m.DateFrom)
            .ThenByDescending(m => m.CreatedDate)
            .ThenBy(m => m.Id)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedList<Mission>(items, total);
    }

    public async Task<IReadOnlyList<Mission>> GetApprovedOverlappingAsync(
        IReadOnlyCollection<Guid> staffIds, DateOnly from, DateOnly to, CancellationToken cancellationToken)
    {
        var ids = staffIds.ToList();
        return await _context.Set<Mission>()
            .Where(m => ids.Contains(m.StaffId)
                        && m.Status == RequestStatus.Approved
                        && m.DateFrom <= to && m.DateTo >= from)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> HasOverlapAsync(
        Guid staffId, DateOnly from, DateOnly to, Guid? excludeId, CancellationToken cancellationToken) =>
        await _context.Set<Mission>().AnyAsync(m =>
            m.StaffId == staffId
            && m.Status != RequestStatus.Rejected
            && m.DateFrom <= to && m.DateTo >= from
            && (!excludeId.HasValue || m.Id != excludeId.Value), cancellationToken);
}
