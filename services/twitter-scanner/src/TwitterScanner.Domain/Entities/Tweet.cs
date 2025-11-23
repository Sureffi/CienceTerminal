namespace TwitterScanner.Domain.Entities;

public class Tweet
{
    public long Id { get; set; }
    public Author Author { get; set; }

    public string Url { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;

    public int ReTweetCount { get; set; }
    public int ReplyCount { get; set; }
    public int LikeCount { get; set; }
    public int QuoteCount { get; set; }
    public int ViewCount { get; set; }
    public int BookmarkCount { get; set; }

    public DateTime CreatedAt { get; set; }
    public string Lang { get; set; } = string.Empty;

    public bool IsReply { get; set; }
    public long? InReplyToId { get; set; }
    public long ConversationId { get; set; }
    public long? InReplyToUserId { get; set; }
    public string InReplyToUsername { get; set; } = string.Empty;

    public bool IsQuote { get; set; }
    public Tweet? QuotedTweet { get; set; }

    public bool IsRetweet { get; set; }
    public Tweet? RetweetedTweet { get; set; }

    public bool IsOriginalPost { get; set; }

    public List<string> MentionedCAs { get; set; } = new();

    public string RuleId { get; set; } = string.Empty;
    public string RuleTag { get; set; } = string.Empty;
}

public class Author
{
    public AuthorType Type { get; set; }
    public long Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string ProfilePicture { get; set; } = string.Empty;
    public bool IsVerified { get; set; }
    public bool IsBlueVerified { get; set; }
    public int FollowerCount { get; set; }
    public int FollowingCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsAutomated { get; set; }
    public int FavouritesCount { get; set; }
}
