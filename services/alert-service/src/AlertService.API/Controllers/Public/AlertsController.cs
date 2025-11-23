using AlertService.Application.Messaging.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AlertService.API.Controllers.Public;

[ApiController]
[Route("api/alerts")]
public class AlertsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AlertsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("twitter")]
    public async Task<IActionResult> GetTwitterAlerts()
    {
        var query = new GetTwitterAlertsQuery();
        var alerts = await _mediator.Send(query);
        return Ok(alerts);
    }

    [HttpGet("ca-mentions")]
    public async Task<IActionResult> GetCaMentionAlerts()
    {
        var query = new GetCaMentionAlertsQuery();
        var alerts = await _mediator.Send(query);
        return Ok(alerts);
    }
}
// TODO: Create DTOs for alerts
// TODO: This could be an internal controller
