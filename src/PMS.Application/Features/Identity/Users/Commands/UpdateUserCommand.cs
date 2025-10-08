using MediatR;
using PMS.Application.Wrappers;

namespace PMS.Application.Features.Identity.Users.Commands;

public class UpdateUserCommand : IRequest<IResponseWrapper>
{
    public UpdateUserRequest UpdateUser { get; set; }
}

public class UpdateUserCommandHandler(IUserService userService) : IRequestHandler<UpdateUserCommand, IResponseWrapper>
{
    public async Task<IResponseWrapper> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var userId = await userService.UpdateAsync(request.UpdateUser);
        return await ResponseWrapper<string>.SuccessAsync(userId, "用户更新成功。");
    }
}
