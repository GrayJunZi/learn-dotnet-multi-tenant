using MediatR;
using PMS.Application.Wrappers;

namespace PMS.Application.Features.Identity.Users.Commands;

public class CreateUserCommand : IRequest<IResponseWrapper>
{
    public CreateUserRequest CreateUser { get; set; }
}

public class CreateUserCommandHandler(IUserService userService) : IRequestHandler<CreateUserCommand, IResponseWrapper>
{
    public async Task<IResponseWrapper> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var userId = await userService.CreateAsync(request.CreateUser);
        return await ResponseWrapper<string>.SuccessAsync(userId, "用户创建成功。");
    }
}