using AlertService.Application.DTOs;
using AlertService.Application.Interfaces;
using MediatR;

namespace AlertService.Application.Messaging.Queries;

/// <summary>
/// Handler for retrieving CA mention records for a specific coin.
/// </summary>
public class GetCaMentionRecordsQueryHandler : IRequestHandler<GetCaMentionRecordsQuery, List<CaMentionRecordDto>>
{
    private readonly ICaMentionRecordRepository _caMentionRecordRepository;

    public GetCaMentionRecordsQueryHandler(ICaMentionRecordRepository caMentionRecordRepository)
    {
        _caMentionRecordRepository = caMentionRecordRepository;
    }

    public async Task<List<CaMentionRecordDto>> Handle(GetCaMentionRecordsQuery request, CancellationToken cancellationToken)
    {
        var mentions = await _caMentionRecordRepository.GetMentionsByCoinAsync(
            request.CoinMintAddress,
            request.Hours,
            request.Limit,
            cancellationToken);

        // Map to DTOs
        var mentionDtos = mentions.Select(m => new CaMentionRecordDto
        {
            Id = m.Id,
            CoinMintAddress = m.CoinMintAddress,
            TweetId = m.TweetId,
            AuthorId = m.AuthorId,
            Username = m.Username,
            ProfilePicture = m.ProfilePicture,
            TweetUrl = m.TweetUrl,
            TweetContent = m.TweetContent,
            Followers = m.Followers,
            IsVerified = m.IsVerified,
            Timestamp = m.Timestamp,
            IsOriginalPost = m.IsOriginalPost,
            IsReply = m.IsReply,
            IsQuote = m.IsQuote,
            IsRetweet = m.IsRetweet
        }).ToList();

        return mentionDtos;
    }
}
