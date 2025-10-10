using MediatR;
using PMS.Core.Wrappers;

namespace PMS.Application.Features.Identity.Users.Commands;

public class ChangeUserStatusCommand : IRequest<IResponseWrapper>
{
    public ChangeUserStatusRequest ChangeUserStatus { get; set; }
}

public class ChangeUserStatusCommandHandler(IUserService userService) : IRequestHandler<ChangeUserStatusCommand, IResponseWrapper>
{
    public async Task<IResponseWrapper> Handle(ChangeUserStatusCommand request, CancellationToken cancellationToken)
    {
        var userId = await userService.ActivateOrDeactivateAsync(request.ChangeUserStatus);
        return await ResponseWrapper<string>.SuccessAsync(userId, request.ChangeUserStatus.Activation ? "用户激活成功。" : "用户禁用成功。");
    }
}
