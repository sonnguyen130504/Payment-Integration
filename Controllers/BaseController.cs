using Microsoft.AspNetCore.Mvc;
using PaymentIntegration.Models;

namespace PaymentIntegration.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseController : ControllerBase
{
    protected ActionResult<ApiResponse<T>> HandleResult<T>(ApiResponse<T> result)
    {
        if (result == null) return NotFound(ApiResponse.ErrorResult(404, "Not Found"));

        if (result.Success)
        {
            return Ok(result);
        }

        return StatusCode(result.StatusCode, result);
    }

    protected ActionResult<ApiResponse> HandleResult(ApiResponse result)
    {
        if (result == null) return NotFound(ApiResponse.ErrorResult(404, "Not Found"));

        if (result.Success)
        {
            return Ok(result);
        }

        return StatusCode(result.StatusCode, result);
    }
}
