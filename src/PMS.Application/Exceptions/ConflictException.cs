using System.Net;

namespace PMS.Application.Exceptions;

public class ConflictException(
    List<string> ErrorMessages = default,
    HttpStatusCode StatusCode = HttpStatusCode.Conflict
    ) : Exception
{

}

