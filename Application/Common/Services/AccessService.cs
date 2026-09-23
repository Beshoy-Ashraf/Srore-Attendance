using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using Domain.Interfaces;

namespace Application.Common.Services;

public sealed class AccessService(IUnitOfWork unitOfWork, ICurrentUser currentUser) : IAccessService
{
    private User? _me;
    private IReadOnlyCollection<Guid>? _visibleStoreIds;
    private bool _visibleLoaded;

    public async Task<User> GetCurrentUserAsync(CancellationToken cancellationToken)
    {
        if (_me is not null)
            return _me;

        if (currentUser.UserId is not { } id)
            throw new UnauthorizedException("You must sign in to continue.");

        _me = await unitOfWork.UserRepository.GetActiveByIdAsync(id, cancellationToken)
            ?? throw new UnauthorizedException("Your account no longer exists.");

        return _me;
    }

    public async Task<IReadOnlyCollection<Guid>?> GetVisibleStoreIdsAsync(CancellationToken cancellationToken)
    {
        if (_visibleLoaded)
            return _visibleStoreIds;

        var me = await GetCurrentUserAsync(cancellationToken);

        IReadOnlyCollection<Guid>? visible;
        if (me.Role == UserRole.Admin)
        {
            visible = null;
        }
        else if (me.Role == UserRole.AreaManager)
        {
            visible = await unitOfWork.StoreRepository.GetIdsByAreaManagerAsync(me.Id, cancellationToken);
        }
        else
        {
            visible = me.StoreId is { } storeId ? new[] { storeId } : Array.Empty<Guid>();
        }

        _visibleStoreIds = visible;
        _visibleLoaded = true;
        return _visibleStoreIds;
    }

    public async Task<IReadOnlyCollection<Guid>?> ResolveStoreScopeAsync(Guid? requestedStoreId, CancellationToken cancellationToken)
    {
        if (requestedStoreId is { } storeId)
        {
            await EnsureStoreAccessAsync(storeId, cancellationToken);
            return new[] { storeId };
        }

        return await GetVisibleStoreIdsAsync(cancellationToken);
    }

    public async Task EnsureRoleAsync(CancellationToken cancellationToken, params UserRole[] roles)
    {
        var me = await GetCurrentUserAsync(cancellationToken);
        if (!roles.Contains(me.Role))
            throw new ForbiddenException("Your role can't do this.");
    }

    public async Task EnsureStoreAccessAsync(Guid storeId, CancellationToken cancellationToken)
    {
        var visible = await GetVisibleStoreIdsAsync(cancellationToken);
        if (visible is not null && !visible.Contains(storeId))
            throw new ForbiddenException("You don't have access to that store.");
    }

    public async Task<User> EnsureStaffAccessAsync(Guid staffId, CancellationToken cancellationToken)
    {
        var me = await GetCurrentUserAsync(cancellationToken);
        if (me.Id == staffId)
            return me;

        var staff = await unitOfWork.UserRepository.GetActiveByIdAsync(staffId, cancellationToken)
            ?? throw new NotFoundException(nameof(User), staffId);

        if (me.Role == UserRole.Staff)
            throw new ForbiddenException("You can only act on your own records.");

        var visible = await GetVisibleStoreIdsAsync(cancellationToken);
        if (visible is null)
            return staff;

        if (staff.StoreId is not { } storeId || !visible.Contains(storeId))
            throw new ForbiddenException("That person isn't in your stores.");

        return staff;
    }

    public async Task<User> EnsureUserVisibleAsync(Guid userId, CancellationToken cancellationToken)
    {
        var me = await GetCurrentUserAsync(cancellationToken);
        if (me.Id == userId)
            return me;

        var user = await unitOfWork.UserRepository.GetActiveByIdAsync(userId, cancellationToken)
            ?? throw new NotFoundException(nameof(User), userId);

        var visible = await GetVisibleStoreIdsAsync(cancellationToken);
        if (visible is null)
            return user;

        if (user.StoreId is { } storeId && visible.Contains(storeId))
            return user;

        // Show who the area manager of the caller's store is, so approvals can name the decider.
        if (user.Role == UserRole.AreaManager && me.StoreId is { } myStoreId)
        {
            var store = await unitOfWork.StoreRepository.GetByIdWithDevicesAsync(myStoreId);
            if (store?.AreaManagerId == user.Id)
                return user;
        }

        throw new ForbiddenException("You don't have access to that user.");
    }

    public async Task EnsureCanManageUserAsync(User target, CancellationToken cancellationToken)
    {
        var me = await GetCurrentUserAsync(cancellationToken);

        if (me.Role == UserRole.Admin)
            return;

        if (me.Role != UserRole.AreaManager)
            throw new ForbiddenException("Only an admin or an area manager can manage accounts.");

        var visible = await GetVisibleStoreIdsAsync(cancellationToken);
        var isInMyStores = target.StoreId is { } storeId && visible is not null && visible.Contains(storeId);

        if (target.Role is not (UserRole.Staff or UserRole.StoreManager) || !isInMyStores)
            throw new ForbiddenException("You can only manage staff and store managers in your own stores.");
    }
}
