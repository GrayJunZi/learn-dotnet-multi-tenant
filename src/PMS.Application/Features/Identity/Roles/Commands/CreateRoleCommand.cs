using MediatR;
using PMS.Core.Wrappers;

namespace PMS.Application.Features.Identity.Roles.Commands;

public class CreateRoleCommand : IRequest<IResponseWrapper>
{
    public CreateRoleRequest CreateRole { get; set; }
}


public class CreateRoleCommandHandler(IRoleService roleService) : IRequestHandler<CreateRoleCommand, IResponseWrapper>
{
    public async Task<IResponseWrapper> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        var roleName = await roleService.CreateAsync(request.CreateRole);
        
        return await ResponseWrapper<string>.SuccessAsync(roleName,$"角色 '{roleName}' 创建成功。");
    }
}
