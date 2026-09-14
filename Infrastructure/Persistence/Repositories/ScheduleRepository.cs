using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class ScheduleRepository(AppDbContext context) : BaseRepository<Schedule>(context), IScheduleRepository
{

    private readonly AppDbContext _context = context;
    public async Task<Schedule?> GetByStaffAndDateAsync(Guid staffId, DateOnly date) =>
          await _context.Set<Schedule>()
              .FirstOrDefaultAsync(s => s.StaffId == staffId && s.Date == date);

    public async Task<IEnumerable<Schedule>> GetByStaffAndDateRangeAsync(Guid staffId, DateOnly from, DateOnly to) =>
        await _context.Set<Schedule>()
            .Where(s => s.StaffId == staffId && s.Date >= from && s.Date <= to)
            .OrderBy(s => s.Date)
            .ToListAsync();

    public async Task<IEnumerable<Schedule>> GetPendingByAreaManagerAsync(Guid areaManagerId) =>
        await _context.Set<Schedule>()
            .Include(s => s.Staff)
            .Where(s => s.Status == ScheduleStatus.Pending && s.Staff.StoreId != null
                        && s.Staff.Store!.AreaManagerId == areaManagerId)
            .ToListAsync();
    public async Task<IEnumerable<Schedule>> GetFilteredAsync(
Guid? staffId, Guid? storeId, DateOnly? from, DateOnly? to, ScheduleStatus? status, int page, int pageSize)
    {
        var query = _context.Set<Schedule>().AsQueryable();

        if (staffId.HasValue)
            query = query.Where(s => s.StaffId == staffId.Value);

        if (storeId.HasValue)
            query = query.Where(s => s.Staff.StoreId == storeId.Value);

        if (from.HasValue)
            query = query.Where(s => s.Date >= from.Value);

        if (to.HasValue)
            query = query.Where(s => s.Date <= to.Value);

        if (status.HasValue)
            query = query.Where(s => s.Status == status.Value);

        return await query
            .OrderBy(s => s.Date)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
}