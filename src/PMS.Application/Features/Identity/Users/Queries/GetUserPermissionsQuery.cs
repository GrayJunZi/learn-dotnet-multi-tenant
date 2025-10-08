using MediatR;
using PMS.Application.Wrappers;

namespace PMS.Application.Features.Identity.Users.Queries;

public class GetUserPermissionsQuery : IRequest<IResponseWrapper>
{
    public string UserId { get; set; }
}

public class GetUserPermissionsQueryHandler(IUserService userService) : IRequestHandler<GetUserPermissionsQuery, IResponseWrapper>
{
    public async Task<IResponseWrapper> Handle(GetUserPermissionsQuery request, CancellationToken cancellationToken)
    {
        var permissions = await userService.GetUserPermissionsAsync(request.UserId, cancellationToken);
        return await ResponseWrapper<List<string>>.SuccessAsync(permissions);
    }
}
