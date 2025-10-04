using System.Net;

namespace PMS.Application.Exceptions;

public class IdentityException(
    List<string> ErrorMessages = default,
    HttpStatusCode StatusCode = HttpStatusCode.InternalServerError) : Exception
{

}

