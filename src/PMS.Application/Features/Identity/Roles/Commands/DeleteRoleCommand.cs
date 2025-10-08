using MediatR;
using PMS.Application.Wrappers;

namespace PMS.Application.Features.Identity.Roles.Commands;

public class DeleteRoleCommand : IRequest<IResponseWrapper>
{
    public string RoleId { get; set; }
}

public class DeleteRoleCommandHandler(IRoleService roleService) : IRequestHandler<DeleteRoleCommand, IResponseWrapper>
{
    public async Task<IResponseWrapper> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
    {
        var roleName = await roleService.DeleteAsync(request.RoleId);
        return await ResponseWrapper<string>.SuccessAsync(roleName, $"角色 '{roleName}' 删除成功。");
    }
}