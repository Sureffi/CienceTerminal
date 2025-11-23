using CienceTerminal.Contracts.Enums;

namespace CienceTerminal.Contracts.Events;

// TODO: Figure out if this structure is good
public class TwitterAlertEvent : AlertEvent
{
    public override AlertType Type { get; }

    // Alert Display
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;

    // Author Information
    public string AuthorName { get; set; } = string.Empty;
    public string AuthorUsername { get; set; } = string.Empty;
    public string AuthorProfilePicture { get; set; } = string.Empty;
    public int AuthorFollowers { get; set; }
    public bool AuthorIsBlueVerified { get; set; }
    public bool AuthorIsGoldVerified { get; set; }

    // Tweet Information
    public long TweetId { get; set; }
    public string TweetContent { get; set; } = string.Empty;
    public string TweetLink { get; set; } = string.Empty;
    public DateTime TweetCreatedAt { get; set; }

    // Token Information
    public string CoinMintAddress { get; set; } = string.Empty;

    public TwitterAlertEvent(AlertType type)
    {
        Type = type;
    }
}
