namespace CienceTerminal.Contracts.Models;

/// <summary>
/// Immutable record of a CA mention event from Twitter.
/// Shared between Twitter Scanner (writes) and Token Metrics Service (reads).
/// This is the raw event log - never modified after insertion.
/// </summary>
public class CaMentionRecord
{
    public Guid Id { get; set; }
    public string CoinMintAddress { get; set; } = string.Empty;
    public string TweetId { get; set; } = string.Empty;
    public string AuthorId { get; set; } = string.Empty; // Twitter user ID
    public string Username { get; set; } = string.Empty;
    public string ProfilePicture { get; set; } = string.Empty;
    public string TweetUrl { get; set; } = string.Empty; // Full URL to the tweet
    public string? TweetContent { get; set; } // Tweet text content (only for original posts)
    public int Followers { get; set; }
    public bool IsVerified { get; set; }
    public DateTime Timestamp { get; set; }

    public bool IsOriginalPost { get; set; } // True if not a reply, quote, or retweet
    public bool IsReply { get; set; }
    public bool IsQuote { get; set; }
    public bool IsRetweet { get; set; }
}
