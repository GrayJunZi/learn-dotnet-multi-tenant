using System.Net;

namespace PMS.Application.Exceptions;

public class NotFoundException(
    List<string> ErrorMessages = default,
    HttpStatusCode StatusCode = HttpStatusCode.NotFound
    ) : Exception
{

}