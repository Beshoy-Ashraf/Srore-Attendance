using Domain.Entities;

namespace Domain.Interfaces;

public interface IAttendanceSettingsRepository : IBaseRepository<AttendanceSettings>
{
      Task<AttendanceSettings?> GetByStoreIdAsync(Guid storeId);
}