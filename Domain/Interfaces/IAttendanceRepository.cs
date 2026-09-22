using Domain.Entities;

namespace Domain.Interfaces;

public interface IAttendanceRepository : IBaseRepository<Attendance>
{
    Task<Attendance?> GetOpenAttendanceAsync(Guid staffId);
    Task<IEnumerable<Attendance>> GetByStaffAndDateRangeAsync(Guid staffId, DateOnly from, DateOnly to);
    Task<IEnumerable<Attendance>> GetByStoreAndDateAsync(Guid storeId, DateOnly date);

    Task<IEnumerable<Attendance>> GetFilteredAsync(
        Guid? staffId, Guid? storeId, DateTime? from, DateTime? to, int page, int pageSize);
}