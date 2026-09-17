using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class RequestRepository(AppDbContext context) : BaseRepository<Request>(context), IRequestRepository
{

    private readonly AppDbContext _context = context;
    public async Task<IEnumerable<Request>> GetPendingByAreaManagerAsync(Guid areaManagerId) =>
          await _context.Set<Request>()
              .Include(r => r.Staff)
              .Where(r => r.Status == RequestStatus.Pending && r.Staff.StoreId != null
                          && r.Staff.Store!.AreaManagerId == areaManagerId)
              .ToListAsync();

    public async Task<IEnumerable<Request>> GetByStaffAsync(Guid staffId) =>
        await _context.Set<Request>()
            .Where(r => r.StaffId == staffId)
            .OrderByDescending(r => r.CreatedDate)
            .ToListAsync();

    public async Task<IEnumerable<Request>> GetFilteredAsync(Guid? staffId, RequestType? type, RequestStatus? status, int page, int pageSize)
    {
        var query = _context.Set<Request>().AsQueryable();

        if (staffId.HasValue)
            query = query.Where(r => r.StaffId == staffId.Value);

        if (type.HasValue)
            query = query.Where(r => r.Type == type.Value);

        if (status.HasValue)
            query = query.Where(r => r.Status == status.Value);

        return await query
            .OrderByDescending(r => r.CreatedDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
}