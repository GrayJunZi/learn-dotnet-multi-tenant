using MediatR;
using PMS.Application.Wrappers;

namespace PMS.Application.Features.Identity.Users.Queries;

public class GetAllUsersQuery : IRequest<IResponseWrapper>
{

}

public class GetAllUsersQueryHandler(IUserService userService) : IRequestHandler<GetAllUsersQuery, IResponseWrapper>
{
    public async Task<IResponseWrapper> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await userService.GetAllAsync();
        return await ResponseWrapper<List<UserResponse>>.SuccessAsync(users);
    }
}
