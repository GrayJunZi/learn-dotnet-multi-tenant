using MediatR;
using PMS.Core.Wrappers;

namespace PMS.Application.Features.Identity.Roles.Commands;

public class UpdateRoleCommand : IRequest<IResponseWrapper>
{
    public UpdateRoleRequest UpdateRole { get; set; }
}

public class UpdateRoleCommandHandler(IRoleService roleService) : IRequestHandler<UpdateRoleCommand, IResponseWrapper>
{
    public async Task<IResponseWrapper> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    {
        var roleName = await roleService.UpdateAsync(request.UpdateRole);

        return await ResponseWrapper<string>.SuccessAsync(roleName, $"角色 '{roleName}' 修改成功。");
    }
}
