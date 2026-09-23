using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Users.Dtos;
using Application.Users.Mappings;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Models;
using MediatR;

namespace Application.Users.Queries.GetUsers;

public class GetUsersQueryHandler(IUnitOfWork unitOfWork, IAccessService access) : IRequestHandler<GetUsersQuery, PagedResult<UserDto>>
{
      public async Task<PagedResult<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
      {
            var me = await access.GetCurrentUserAsync(cancellationToken);
            var (page, pageSize) = Paging.Normalize(request.Page, request.PageSize);

            IReadOnlyCollection<Guid>? storeIds;
            IReadOnlyCollection<Guid>? alsoInclude = null;

            if (request.StoreId is { } requestedStore)
            {
                  await access.EnsureStoreAccessAsync(requestedStore, cancellationToken);
                  storeIds = new[] { requestedStore };
            }
            else
            {
                  storeIds = await access.GetVisibleStoreIdsAsync(cancellationToken);

                  if (storeIds is not null)
                  {
                        // The caller and their store's area manager belong in the directory even though they
                        // are not "in" the store, so approvals can show who decided.
                        var extra = new List<Guid> { me.Id };
                        if (me.Role != UserRole.AreaManager && me.StoreId is { } myStoreId)
                        {
                              var store = await unitOfWork.StoreRepository.GetByIdWithDevicesAsync(myStoreId);
                              if (store?.AreaManagerId is { } areaManagerId)
                                    extra.Add(areaManagerId);
                        }

                        alsoInclude = extra;
                  }
            }

            var filter = new UserFilter
            {
                  StoreIds = storeIds,
                  AlsoIncludeUserIds = alsoInclude,
                  Role = request.Role,
                  Search = request.Search,
                  Page = page,
                  PageSize = pageSize
            };

            var result = await unitOfWork.UserRepository.GetPagedAsync(filter, cancellationToken);
            return PagedResult<UserDto>.From(result, u => u.ToDto(), page, pageSize);
      }
}
