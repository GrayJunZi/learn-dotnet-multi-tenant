using Microsoft.AspNetCore.Http;
using PMS.Application.Features.Identity.Users;

namespace PMS.Infrastructure.Identity;

public class CurrentUserMiddleware(ICurrentUserService currentUserService) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        currentUserService.SetCurrentUser(context.User);
        await next(context);
    }
}
