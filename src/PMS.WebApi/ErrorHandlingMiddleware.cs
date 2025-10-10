using PMS.Application.Exceptions;
using PMS.Core.Wrappers;
using System.Net;

namespace PMS.WebApi;

public class ErrorHandlingMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            var response = context.Response;
            response.ContentType = "application/json";

            var responseWrapper = ResponseWrapper.Fail();

            switch (ex)
            {
                case ConflictException ce:
                    response.StatusCode = (int)ce.StatusCode;
                    responseWrapper.Messages = ce.ErrorMessages;
                    break;
                case NotFoundException nfe:
                    response.StatusCode = (int)nfe.StatusCode;
                    responseWrapper.Messages = nfe.ErrorMessages;
                    break;
                case ForbiddenException fe:
                    response.StatusCode = (int)fe.StatusCode;
                    responseWrapper.Messages = fe.ErrorMessages;
                    break;
                case IdentityException ie:
                    response.StatusCode = (int)ie.StatusCode;
                    responseWrapper.Messages = ie.ErrorMessages;
                    break;
                case UnauthorizedException ue:
                    response.StatusCode = (int)ue.StatusCode;
                    responseWrapper.Messages = ue.ErrorMessages;
                    break;
                default:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    responseWrapper.Messages = [ex.Message];
                    break;
            }

            await response.WriteAsJsonAsync(responseWrapper);
        }
    }
}
