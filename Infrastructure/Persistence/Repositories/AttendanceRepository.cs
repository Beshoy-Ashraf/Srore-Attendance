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
}