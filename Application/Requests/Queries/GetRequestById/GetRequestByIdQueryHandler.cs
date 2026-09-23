using Application.Common.Interfaces;
using Application.Requests.Dtos;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using MediatR;

namespace Application.Requests.Queries.GetRequestById;

public class GetRequestByIdQueryHandler(IUnitOfWork unitOfWork, IAccessService access)
    : IRequestHandler<GetRequestByIdQuery, RequestDto>
{
      public async Task<RequestDto> Handle(GetRequestByIdQuery request, CancellationToken cancellationToken)
      {
            var entity = await unitOfWork.RequestRepository.GetDetailedByIdAsync(request.Id, includeDeleted: true, cancellationToken)
                ?? throw new NotFoundException(nameof(Request), request.Id);

            await access.EnsureStaffAccessAsync(entity.StaffId, cancellationToken);

            return entity.ToDto();
      }
}
