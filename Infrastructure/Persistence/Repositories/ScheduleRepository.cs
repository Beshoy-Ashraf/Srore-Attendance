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
}