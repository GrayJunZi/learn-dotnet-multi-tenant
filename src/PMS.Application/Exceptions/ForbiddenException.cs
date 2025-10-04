using System.Net;

namespace PMS.Application.Exceptions;

public class ForbiddenException(
    List<string> errorMessages = default,
    HttpStatusCode StatusCode = HttpStatusCode.Forbidden
    ) : Exception
{

}

