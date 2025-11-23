namespace TwitterScanner.Domain.Entities;

// TODO: Make better format for this?
// Could contain full Author info
public class CaMention
{
    public string TweetId { get; private set; }
    public string AuthorId { get; private set; }
    public string Username { get; private set; }
    public string ProfilePicture { get; private set; }
    public int FollowerCount { get; private set; }
    public bool IsVerified { get; private set; }

    public string CoinMintAddress { get; private set; }
    public string TweetUrl { get; private set; }
    public string? TweetContent { get; private set; }
    public bool IsOriginalPost { get; private set; }
    public DateTime Timestamp { get; set; }
    public bool IsReply { get; set; }
    public bool IsQuote { get; set; }
    public bool IsRetweet { get; set; }

    public CaMention(Tweet tweet, string coinMintAddress)
    {
        TweetId = tweet.Id.ToString();
        AuthorId = tweet.Author.Id.ToString();
        Username = tweet.Author.UserName;
        ProfilePicture = tweet.Author.ProfilePicture;
        FollowerCount = tweet.Author.FollowerCount;
        IsVerified = tweet.Author.IsBlueVerified;

        Timestamp = DateTime.UtcNow;
        CoinMintAddress = coinMintAddress;
        TweetUrl = tweet.Url;
        IsOriginalPost = tweet.IsOriginalPost;
        // Store content for original posts, replies, and quotes (but not pure retweets)
        TweetContent = !tweet.IsRetweet ? tweet.Content : null;
        IsReply = tweet.IsReply;
        IsQuote = tweet.IsQuote;
        IsRetweet = tweet.IsRetweet;
    }
}
