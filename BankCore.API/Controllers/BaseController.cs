using BankCore.Application.Common;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BankCore.API.Controllers;

[ApiController]
public class BaseController : ControllerBase
{
    protected IActionResult HandleResult<T>(Result<T> result)
    {
        if (result.IsSuccess)
            return Ok(result.Value);

        return HandleFailure(result);
    }

    protected IActionResult HandleResult(Result result)
    {
        if (result.IsSuccess)
            return Ok(result);

        return HandleFailure(result);
    }

    private IActionResult HandleFailure(Result result)
    {
        var statusCode = result.ErrorType switch
        {
            ErrorType.NotFound => 404,
            ErrorType.Unauthorized => 401,
            ErrorType.Forbidden => 403,
            ErrorType.Conflict => 409,
            ErrorType.Validation => 400,
            _ => 500
        };

        return Problem(statusCode: statusCode, detail: result.Error);
    }

    protected int GetUserId()
    {
        return int.Parse(
            User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
    }
}