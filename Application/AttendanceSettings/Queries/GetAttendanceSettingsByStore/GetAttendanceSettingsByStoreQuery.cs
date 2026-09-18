using Application.AttendanceSettings.Dtos;
using MediatR;

namespace Application.AttendanceSettings.Queries.GetAttendanceSettingsByStore;

public record GetAttendanceSettingsByStoreQuery(Guid StoreId) : IRequest<AttendanceSettingsDto>;