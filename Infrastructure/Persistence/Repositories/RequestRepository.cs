using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class RequestRepository(AppDbContext context) : BaseRepository<Request>(context), IRequestRepository
{
    private readonly AppDbContext _context = context;

    private static IQueryable<Request> WithDetails(IQueryable<Request> query) =>
        query
            .Include(r => r.Staff).ThenInclude(u => u.Store)
            .Include(r => r.RequestedBy)
            .Include(r => r.ApprovedByAreaManager);

    public async Task<Request?> GetDetailedByIdAsync(Guid id, CancellationToken cancellationToken) =>
        await GetDetailedByIdAsync(id, includeDeleted: false, cancellationToken);

    public async Task<Request?> GetDetailedByIdAsync(Guid id, bool includeDeleted, CancellationToken cancellationToken)
    {
        IQueryable<Request> query = _context.Set<Request>();
        if (includeDeleted)
            query = query.IgnoreQueryFilters();

        return await WithDetails(query).FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<PagedList<Request>> GetPagedAsync(RequestFilter filter, CancellationToken cancellationToken)
    {
        // A rejected request is soft-deleted; asking for "Rejected" is how the history is read.
        IQueryable<Request> query = filter.Status == RequestStatus.Rejected
            ? _context.Set<Request>().IgnoreQueryFilters()
            : _context.Set<Request>();

        query = WithDetails(query);

        if (filter.StaffId.HasValue)
            query = query.Where(r => r.StaffId == filter.StaffId.Value);

        if (filter.StoreIds is not null)
        {
            var storeIds = filter.StoreIds.ToList();
            query = query.Where(r => r.Staff.StoreId != null && storeIds.Contains(r.Staff.StoreId.Value));
        }

        if (filter.Type.HasValue)
            query = query.Where(r => r.Type == filter.Type.Value);

        if (filter.Status.HasValue)
            query = query.Where(r => r.Status == filter.Status.Value);

        if (filter.From.HasValue)
            query = query.Where(r => r.DateTo >= filter.From.Value);

        if (filter.To.HasValue)
            query = query.Where(r => r.DateFrom <= filter.To.Value);

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(r => r.DateFrom)
            .ThenByDescending(r => r.CreatedDate)
            .ThenBy(r => r.Id)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedList<Request>(items, total);
    }

    public async Task<bool> HasOverlapAsync(
        Guid staffId, RequestType type, DateOnly from, DateOnly to, Guid? excludeId, CancellationToken cancellationToken) =>
        await _context.Set<Request>().AnyAsync(r =>
            r.StaffId == staffId
            && r.Type == type
            && r.Status != RequestStatus.Rejected
            && r.DateFrom <= to && r.DateTo >= from
            && (!excludeId.HasValue || r.Id != excludeId.Value), cancellationToken);
}
