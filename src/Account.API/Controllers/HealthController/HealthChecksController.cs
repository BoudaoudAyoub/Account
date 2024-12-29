using Microsoft.AspNetCore.Mvc;
namespace Account.API.Controllers.HealthController;

[ApiController]
[Route("[Controller]")]
public class HealthChecksController(ILogger<HealthChecksController> logger) : ControllerBase
{
    public readonly ILogger<HealthChecksController> _logger = logger;

    [HttpGet]
    public async Task<object> GetHelloWorld() => await Task.FromResult("Hello World");
}