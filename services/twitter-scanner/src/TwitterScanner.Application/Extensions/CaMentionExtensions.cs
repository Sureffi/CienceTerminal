using CienceTerminal.Contracts.Models;
using TwitterScanner.Domain.Entities;

namespace TwitterScanner.Application.Extensions;

/// <summary>
/// Extension methods for CaMentions
/// </summary>
public static class CaMentionExtensions
{

    /// <summary>
    /// Maps a CaMention to a CaMentionRecord
    /// </summary>
    /// <param name="caMention"></param>
    /// <returns></returns>
    public static CaMentionRecord ToCaMentionRecord(this CaMention caMention)
    {
        return new CaMentionRecord
        {
            Id = Guid.NewGuid(),
            CoinMintAddress = caMention.CoinMintAddress,
            TweetId = caMention.TweetId,
            AuthorId = caMention.AuthorId,
            Username = caMention.Username,
            ProfilePicture = caMention.ProfilePicture,
            TweetUrl = caMention.TweetUrl,
            TweetContent = caMention.TweetContent,
            Followers = caMention.FollowerCount,
            IsVerified = caMention.IsVerified,
            Timestamp = caMention.Timestamp,

            IsOriginalPost = caMention.IsOriginalPost,
            IsReply = caMention.IsReply,
            IsQuote = caMention.IsQuote,
            IsRetweet = caMention.IsRetweet
        };
    }
}
