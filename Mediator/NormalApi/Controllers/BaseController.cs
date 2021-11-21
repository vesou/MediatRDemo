using Microsoft.AspNetCore.Mvc;

namespace NormalApi.Controllers
{
    public abstract class BaseController : ControllerBase
    {
        internal ActionResult OkOrNotFound(object value)
        {
            return value is null ? new NotFoundResult() : new OkObjectResult(value);
        }
    }
}