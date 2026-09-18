using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class MissionRepository(AppDbContext context) : BaseRepository<Mission>(context), IMissionRepository
{
    private readonly AppDbContext _context = context;

    public async Task<IEnumerable<Mission>> GetPendingByAreaManagerAsync(Guid areaManagerId) =>
          await _context.Set<Mission>()
              .Include(m => m.Staff)
              .Where(m => m.Status == RequestStatus.Pending && m.Staff.StoreId != null
                          && m.Staff.Store!.AreaManagerId == areaManagerId)
              .ToListAsync();

    public async Task<IEnumerable<Mission>> GetByStaffAsync(Guid staffId) =>
        await _context.Set<Mission>()
            .Where(m => m.StaffId == staffId)
            .OrderByDescending(m => m.CreatedDate)
            .ToListAsync();
    public async Task<IEnumerable<Mission>> GetFilteredAsync(Guid? staffId, RequestStatus? status, int page, int pageSize)
    {
        var query = _context.Set<Mission>().AsQueryable();

        if (staffId.HasValue)
            query = query.Where(m => m.StaffId == staffId.Value);

        if (status.HasValue)
            query = query.Where(m => m.Status == status.Value);

        return await query
            .OrderByDescending(m => m.CreatedDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
}