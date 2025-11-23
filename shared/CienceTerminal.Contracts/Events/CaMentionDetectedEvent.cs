namespace CienceTerminal.Contracts.Events;

/// <summary>
/// Simple raw event published when a CA mention is detected in a tweet.
/// No trending logic - just the facts about the mention.
/// </summary>
public class CaMentionDetectedEvent
{
    public Guid Id { get; set; }
    public DateTime Timestamp { get; set; }
    public string CoinMintAddress { get; set; } = string.Empty;

    public string TweetId { get; set; } = string.Empty;
    public string AuthorId { get; set; } = string.Empty; // Twitter user ID
    public string Username { get; set; } = string.Empty;
    public string ProfilePicture { get; set; } = string.Empty;
    public string TweetUrl { get; set; } = string.Empty; // Full URL to the tweet
    public string? TweetContent { get; set; } // Tweet text content (only for original posts)
    public int Followers { get; set; }
    public bool IsVerified { get; set; }
    public bool IsOriginalPost { get; set; } // True if not a reply, quote, or retweet
    public bool IsReply { get; set; }
    public bool IsQuote { get; set; }
    public bool IsRetweet { get; set; }
}
