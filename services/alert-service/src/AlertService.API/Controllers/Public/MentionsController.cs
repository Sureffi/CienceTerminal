using AlertService.Application.Messaging.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AlertService.API.Controllers.Public;

[ApiController]
[Route("api/mentions")]
public class MentionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public MentionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Gets CA mention records for a specific coin address.
    /// </summary>
    /// <param name="coinAddress">The coin's mint address (Solana contract address)</param>
    /// <param name="hours">Number of hours to look back (default 24)</param>
    /// <returns>List of CA mention records</returns>
    [HttpGet("{coinAddress}")]
    public async Task<IActionResult> GetMentionsByCoin(
        [FromRoute] string coinAddress,
        [FromQuery] int hours = 24)
    {
        var query = new GetCaMentionRecordsQuery
        {
            CoinMintAddress = coinAddress,
            Hours = hours,
            Limit = 9
        };

        var mentions = await _mediator.Send(query);
        return Ok(mentions);
    }
}
