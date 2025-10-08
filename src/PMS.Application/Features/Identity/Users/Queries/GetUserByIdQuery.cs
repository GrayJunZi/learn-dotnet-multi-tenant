using MediatR;
using PMS.Application.Wrappers;

namespace PMS.Application.Features.Identity.Users.Queries;

public class GetUserByIdQuery : IRequest<IResponseWrapper>
{
    public string UserId { get; set; }
}

public class GetUserByIdQueryHandler(IUserService userService) : IRequestHandler<GetUserByIdQuery, IResponseWrapper>
{
    public async Task<IResponseWrapper> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await userService.GetByIdAsync(request.UserId, cancellationToken);
        return await ResponseWrapper<UserResponse>.SuccessAsync(user);
    }
}
