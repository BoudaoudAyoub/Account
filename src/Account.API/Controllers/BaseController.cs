using Microsoft.AspNetCore.Mvc;
namespace Account.API.Controllers;
[ApiController]
[Route("[Controller]")]
public abstract class BaseController : ControllerBase
{
    protected BaseController() { }
}