using MediatR;
using PMS.Core.Wrappers;

namespace PMS.Application.Features.Identity.Roles.Queries;

public class GetRolesQuery : IRequest<IResponseWrapper>
{

}

public class GetRolesQueryHandler(IRoleService roleService) : IRequestHandler<GetRolesQuery, IResponseWrapper>
{
    public async Task<IResponseWrapper> Handle(GetRolesQuery request, CancellationToken cancellationToken)
    {
        var roles = await roleService.GetAllAsync(cancellationToken);
        return await ResponseWrapper<List<RoleResponse>>.SuccessAsync(roles);
    }
}
