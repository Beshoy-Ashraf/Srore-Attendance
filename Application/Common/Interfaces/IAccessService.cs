using Domain.Entities;
using Domain.Enums;

namespace Application.Common.Interfaces;

/// <summary>
/// Resource-level authorisation. Controllers gate by role; this service decides *which* stores, staff
/// and records the signed-in user may touch:
/// Admin sees everything, an Area Manager sees the stores assigned to them, a Store Manager and Staff
/// see their own store (Staff only act on their own records).
/// </summary>
public interface IAccessService
{
    /// <summary>The signed-in, active user (re-read from the database so role or store changes apply immediately).</summary>
    Task<User> GetCurrentUserAsync(CancellationToken cancellationToken);

    /// <summary>Store ids the caller may see. <c>null</c> means every store (Admin).</summary>
    Task<IReadOnlyCollection<Guid>?> GetVisibleStoreIdsAsync(CancellationToken cancellationToken);

    /// <summary>Narrows to <paramref name="requestedStoreId"/> after checking access, otherwise returns the visible stores.</summary>
    Task<IReadOnlyCollection<Guid>?> ResolveStoreScopeAsync(Guid? requestedStoreId, CancellationToken cancellationToken);

    Task EnsureRoleAsync(CancellationToken cancellationToken, params UserRole[] roles);

    Task EnsureStoreAccessAsync(Guid storeId, CancellationToken cancellationToken);

    /// <summary>The staff member whose records the caller wants to read or change. Staff can only act on themselves.</summary>
    Task<User> EnsureStaffAccessAsync(Guid staffId, CancellationToken cancellationToken);

    /// <summary>A user the caller may look up (self, same store, or their area manager).</summary>
    Task<User> EnsureUserVisibleAsync(Guid userId, CancellationToken cancellationToken);

    /// <summary>Whether the caller may edit, reset or delete <paramref name="target"/>'s account.</summary>
    Task EnsureCanManageUserAsync(User target, CancellationToken cancellationToken);
}
