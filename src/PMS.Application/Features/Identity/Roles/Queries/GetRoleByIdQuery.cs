using MediatR;
using PMS.Application.Wrappers;

namespace PMS.Application.Features.Identity.Roles.Queries;

public class GetRoleByIdQuery : IRequest<IResponseWrapper>
{
    public string RoleId { get; set; }
}

public class GetRoleByIdQueryHandler(IRoleService roleService) : IRequestHandler<GetRoleByIdQuery, IResponseWrapper>
{
    public async Task<IResponseWrapper> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
    {
        var role = await roleService.GetByIdAsync(request.RoleId, cancellationToken);
        return await ResponseWrapper<RoleResponse>.SuccessAsync(role);
    }
}
