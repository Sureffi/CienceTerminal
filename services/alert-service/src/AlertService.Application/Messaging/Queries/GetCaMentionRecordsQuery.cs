using AlertService.Application.DTOs;
using MediatR;

namespace AlertService.Application.Messaging.Queries;

/// <summary>
/// Query to retrieve CA mention records for a specific coin.
/// </summary>
public class GetCaMentionRecordsQuery : IRequest<List<CaMentionRecordDto>>
{
    public string CoinMintAddress { get; set; } = string.Empty;
    public int Hours { get; set; } = 24;
    public int Limit { get; set; } = 9;
}
