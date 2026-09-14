using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class AttendanceRepository(AppDbContext context) : BaseRepository<Attendance>(context), IAttendanceRepository
{
      private readonly AppDbContext _context = context;

      public async Task<Attendance?> GetOpenAttendanceAsync(Guid staffId) =>
            await _context.Set<Attendance>()
                .Where(a => a.StaffId == staffId && a.CheckInTime != null && a.CheckOutTime == null)
                .OrderByDescending(a => a.CheckInTime)
                .FirstOrDefaultAsync();

      public async Task<IEnumerable<Attendance>> GetByStaffAndDateRangeAsync(Guid staffId, DateOnly from, DateOnly to)
      {
            var fromDate = from.ToDateTime(TimeOnly.MinValue);
            var toDate = to.ToDateTime(TimeOnly.MaxValue);

            return await _context.Set<Attendance>()
                .Where(a => a.StaffId == staffId && a.CheckInTime >= fromDate && a.CheckInTime <= toDate)
                .OrderBy(a => a.CheckInTime)
                .ToListAsync();
      }

      public async Task<IEnumerable<Attendance>> GetByStoreAndDateAsync(Guid storeId, DateOnly date)
      {
            var dayStart = date.ToDateTime(TimeOnly.MinValue);
            var dayEnd = date.ToDateTime(TimeOnly.MaxValue);

            return await _context.Set<Attendance>()
                .Include(a => a.Staff)
                .Where(a => a.Staff.StoreId == storeId && a.CheckInTime >= dayStart && a.CheckInTime <= dayEnd)
                .ToListAsync();
      }
      public async Task<IEnumerable<Attendance>> GetFilteredAsync(
        Guid? staffId, Guid? storeId, DateOnly? from, DateOnly? to, int page, int pageSize)
      {
            var query = _context.Set<Attendance>().Include(a => a.Staff).AsQueryable();

            if (staffId.HasValue)
                  query = query.Where(a => a.StaffId == staffId.Value);

            if (storeId.HasValue)
                  query = query.Where(a => a.Staff.StoreId == storeId.Value);

            if (from.HasValue)
            {
                  var fromDate = from.Value.ToDateTime(TimeOnly.MinValue);
                  query = query.Where(a => a.CheckInTime >= fromDate);
            }

            if (to.HasValue)
            {
                  var toDate = to.Value.ToDateTime(TimeOnly.MaxValue);
                  query = query.Where(a => a.CheckInTime <= toDate);
            }

            return await query
                .OrderByDescending(a => a.CheckInTime)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
      }
}