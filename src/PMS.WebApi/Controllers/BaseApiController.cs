﻿using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace PMS.WebApi.Controllers;

[ApiController]
public class BaseApiController : ControllerBase
{
    private ISender _sender;

    public ISender Sender => _sender ??= HttpContext.RequestServices
        .GetRequiredService<ISender>();
}