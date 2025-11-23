using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.API.Controllers;

/// <summary>
/// CA (Contract Address) mention details endpoints - proxies to Twitter Scanner Service
/// </summary>
[ApiController]
[Route("api/v1/ca-mentions")]
[Produces("application/json")]
public class CaMentionsController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<CaMentionsController> _logger;

    public CaMentionsController(IHttpClientFactory httpClientFactory, ILogger<CaMentionsController> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    /// <summary>
    /// Get CA mention details by contract address
    /// </summary>
    /// <param name="address">The contract address to lookup</param>
    /// <param name="hours">Number of hours to look back (1-168, default: 24)</param>
    /// <returns>CA mention details including tweets and statistics</returns>
    /// <response code="200">Returns the CA mention details</response>
    /// <response code="400">If the address is invalid or hours parameter is out of range</response>
    [HttpGet("{address}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetCaMentionDetails(
        [FromRoute] string address,
        [FromQuery] int hours = 24)
    {
        if (string.IsNullOrWhiteSpace(address))
        {
            return BadRequest(new { error = "Contract address is required" });
        }

        if (hours is <= 0 or > 168)
        {
            return BadRequest(new { error = "Hours must be between 1 and 168" });
        }

        try
        {
            var client = _httpClientFactory.CreateClient("TwitterScanner");
            var response = await client.GetAsync($"api/ca-mentions/{address}?hours={hours}");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to fetch CA mention details for {Address}: {StatusCode}",
                    address, response.StatusCode);
                return StatusCode((int)response.StatusCode);
            }

            var content = await response.Content.ReadAsStringAsync();
            return Content(content, "application/json");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching CA mention details for {Address}", address);
            return StatusCode(500, new { error = "Failed to fetch CA mention details" });
        }
    }
}
