using System.Net;

namespace PMS.Application.Exceptions;

public class UnauthorizedException(
    List<string> ErrorMessages = default,
    HttpStatusCode StatusCode = HttpStatusCode.Unauthorized
    ) : Exception
{

}
