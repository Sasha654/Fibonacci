using Microsoft.AspNetCore.Mvc;

namespace PT.Fibonacci.SecondApp.Controllers;

[Route("[controller]")]
[ApiController]
public sealed class CalculationController : ControllerBase
{
    private readonly ILogger<CalculationController> _logger;
    private readonly IActorBridge _actorBridge;

    public CalculationController(ILogger<CalculationController> logger, IActorBridge actorBridge)
    {
        _logger = logger;
        _actorBridge = actorBridge;
    }

    [HttpPost]
    public void Post([FromBody] FibNumRequest request)
    {
        _logger.LogInformation("Received calc request with id [{Id}]", request.CalculationId);
        _actorBridge.Tell(request);
    }
}