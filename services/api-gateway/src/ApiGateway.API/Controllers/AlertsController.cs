using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.API.Controllers;

/// <summary>
/// Alert management endpoints - proxies to Alert Service
/// </summary>
[ApiController]
[Route("api/v1/alerts")]
[Produces("application/json")]
public class AlertsController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<AlertsController> _logger;

    public AlertsController(IHttpClientFactory httpClientFactory, ILogger<AlertsController> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    /// <summary>
    /// Get all Twitter alerts
    /// </summary>
    /// <returns>List of Twitter alerts</returns>
    /// <response code="200">Returns the list of Twitter alerts</response>
    [HttpGet("twitter")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTwitterAlerts()
    {
        try
        {
            var client = _httpClientFactory.CreateClient("AlertService");
            var response = await client.GetAsync("api/alerts/twitter");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to fetch Twitter alerts: {StatusCode}", response.StatusCode);
                return StatusCode((int)response.StatusCode);
            }

            var content = await response.Content.ReadAsStringAsync();
            return Content(content, "application/json");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching Twitter alerts");
            return StatusCode(500, new { error = "Failed to fetch Twitter alerts" });
        }
    }

    /// <summary>
    /// Get all CA mention alerts
    /// </summary>
    /// <returns>List of CA mention alerts</returns>
    /// <response code="200">Returns the list of CA mention alerts</response>
    [HttpGet("ca-mentions")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCaMentionAlerts()
    {
        try
        {
            var client = _httpClientFactory.CreateClient("AlertService");
            var response = await client.GetAsync("api/alerts/ca-mentions");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to fetch CA mention alerts: {StatusCode}", response.StatusCode);
                return StatusCode((int)response.StatusCode);
            }

            var content = await response.Content.ReadAsStringAsync();
            return Content(content, "application/json");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching CA mention alerts");
            return StatusCode(500, new { error = "Failed to fetch CA mention alerts" });
        }
    }
}
