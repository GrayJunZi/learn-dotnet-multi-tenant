using MediatR;
using PMS.Core.Wrappers;

namespace PMS.Application.Features.Identity.Roles.Queries;


public class GetRoleWithPermissionQuery : IRequest<IResponseWrapper>
{
    public string RoleId { get; set; }
}

public class GetRoleWithPermissionQueryHandler(IRoleService roleService) : IRequestHandler<GetRoleWithPermissionQuery, IResponseWrapper>
{
    public async Task<IResponseWrapper> Handle(GetRoleWithPermissionQuery request, CancellationToken cancellationToken)
    {
        var role = await roleService.GetRoleWithPermissionsAsync(request.RoleId, cancellationToken);
        return await ResponseWrapper<RoleResponse>.SuccessAsync(role);
    }
}
