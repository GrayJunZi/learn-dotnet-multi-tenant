﻿using System.Net;

namespace PMS.Application.Exceptions;

public class UnauthorizedException : Exception
{
    public List<string> ErrorMessages { get; set; }
    public HttpStatusCode StatusCode { get; set; }
    public UnauthorizedException(List<string> errorMessages = default, HttpStatusCode statusCode = HttpStatusCode.Unauthorized)
    {
        ErrorMessages = errorMessages;
        StatusCode = statusCode;
    }
}
