using Domain.Entities;

namespace Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
      IBaseRepository<User> Users { get; }
      IUserRepository UserRepository { get; }
      IStoreRepository StoreRepository { get; }
      IScheduleRepository ScheduleRepository { get; }
      IAttendanceRepository AttendanceRepository { get; }
      IRequestRepository RequestRepository { get; }
      IMissionRepository MissionRepository { get; }
      IDeviceRepository DeviceRepository { get; }
      IAttendanceSettingsRepository AttendanceSettingsRepository { get; }


      Task<int> Complete(CancellationToken cancellationToken);
}
