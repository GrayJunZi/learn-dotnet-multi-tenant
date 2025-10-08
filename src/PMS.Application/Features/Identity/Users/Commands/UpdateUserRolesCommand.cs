using MediatR;
using PMS.Application.Wrappers;

namespace PMS.Application.Features.Identity.Users.Commands;

public class UpdateUserRolesCommand : IRequest<IResponseWrapper>
{
    public string UserId { get; set; }
    public UserRolesRequest UserRoles { get; set; }
}

public class UpdateUserRolesCommandHandler(IUserService userService) : IRequestHandler<UpdateUserRolesCommand, IResponseWrapper>
{
    public async Task<IResponseWrapper> Handle(UpdateUserRolesCommand request, CancellationToken cancellationToken)
    {
        var userId = await userService.AssignRolesAsync(request.UserRoles);
        return await ResponseWrapper<string>.SuccessAsync(userId, "用户角色更新成功。");
    }
}
