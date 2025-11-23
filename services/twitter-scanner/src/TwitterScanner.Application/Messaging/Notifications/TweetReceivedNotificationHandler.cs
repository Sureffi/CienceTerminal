using CienceTerminal.Contracts.Enums;
using CienceTerminal.Contracts.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using TwitterScanner.Application.Messaging.Commands;
using TwitterScanner.Application.Messaging.Requests;
using TwitterScanner.Application.Utils;
using TwitterScanner.Domain.Entities;
using TwitterScanner.Domain.Enums;
using TwitterScanner.Domain.Events;
using TwitterScanner.Domain.Groq.Prompts;

namespace TwitterScanner.Application.Messaging.Notifications;

public class TweetReceivedNotificationHandler : INotificationHandler<TweetReceivedNotification>
{
    private readonly IMediator _mediator;
    private readonly ILogger<TweetReceivedNotificationHandler> _logger;

    public TweetReceivedNotificationHandler(IMediator mediator, ILogger<TweetReceivedNotificationHandler> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task Handle(TweetReceivedNotification notification, CancellationToken cancellationToken)
    {
        List<string> coinAddresses;

        coinAddresses = new List<string>();

        // Always extract ca from original tweet content
        coinAddresses.AddRange(TweetUtils.TryExtractCa(notification.Tweet.Content));

        if (notification.Tweet.IsRetweet && notification.Tweet.RetweetedTweet?.Content is not null) // If Tweet is retweet and retweeted tweet is not null
        {
            // Extract ca from retweeted tweets content
            coinAddresses.AddRange(TweetUtils.TryExtractCa(notification.Tweet.RetweetedTweet.Content));
        }
        else if (notification.Tweet.IsQuote && notification.Tweet.QuotedTweet?.Content is not null) // If tweet is quote and quoted tweet is not null
        {
            // Extract ca from quoted tweets content
            coinAddresses.AddRange(TweetUtils.TryExtractCa(notification.Tweet.QuotedTweet.Content));
        }

        // Remove duplicates
        coinAddresses = coinAddresses.Distinct().ToList();

        // skip if no CAs found
        if (coinAddresses.Count == 0) return;

        // Verify extracted CAs
        List<string> verifiedTokens = new();
        foreach (string ca in coinAddresses)
        {
            _logger.LogInformation("Verifying token with rpc");
            // TODO: Cache rpc results for verifying coins

            var tokenInfo = await _mediator.Send(new GetParsedTokenMintDataRequest(ca), cancellationToken);

            // If token info is not null and freeze & mint authority is disabled
            if (tokenInfo != null && string.IsNullOrWhiteSpace(tokenInfo.Info.FreezeAuthority) && string.IsNullOrWhiteSpace(tokenInfo.Info.FreezeAuthority) && tokenInfo.Info.IsInitialized)
            {
                verifiedTokens.Add(ca);
            }
        }

        // If verified token count not 0
        // Send ca mention notification
        if (verifiedTokens.Count != 0)
        {
            foreach (var token in verifiedTokens)
            {
                await _mediator.Publish(new CaMentionReceivedNotification(new CaMention(notification.Tweet, token)), cancellationToken);
            }
        }

        // Disable ai temporarily
        // return;

        // Skip rest of processing if not an original post
        if (!notification.Tweet.IsOriginalPost)
        {
            return;
        }

        // Skip if no verification on authors account
        if (!notification.Tweet.Author.IsBlueVerified && !notification.Tweet.Author.IsVerified)
        {
            return;
        }

        // If tweet mentions one real coin, classify with ai
        if (verifiedTokens.Count == 1)
        {
            // TODO: Ai classification
            TweetClassifierResult classifierResult = await _mediator.Send(new ClassifyTweetRequest(notification.Tweet), cancellationToken);

            AlertType alertType = classifierResult.TweetClass == TweetClass.Legit ? AlertType.TwitterLegit : AlertType.TwitterSpam;

            // Create and publish Twitter alert event
            // TODO: Fix parameters
            if (classifierResult.TweetClass == TweetClass.Legit)
            {
                var alertEvent = CreateTwitterAlertEvent(notification.Tweet, alertType, verifiedTokens.First());
                await _mediator.Send(new PublishTwitterAlertCommand(alertEvent), cancellationToken);
            }
        }
    }

    /// <summary>
    /// Helper method for creating a shared TwitterAlertEvent
    /// </summary>
    private static TwitterAlertEvent CreateTwitterAlertEvent(
        Tweet tweet,
        AlertType alertType,
        string coinMint
        )
    {
        return new TwitterAlertEvent(alertType)
        {
            // Generate deterministic alert ID from tweet ID to prevent duplicates
            AlertId = GenerateAlertIdFromTweetId(tweet.Id),
            Severity = DetermineAlertSeverity(tweet, alertType),
            Title = GenerateAlertTitle(tweet, alertType),
            Message = GenerateAlertMessage(tweet, alertType),

            // Author Information
            AuthorName = tweet.Author.Name,
            AuthorUsername = tweet.Author.UserName,
            AuthorProfilePicture = tweet.Author.ProfilePicture,
            AuthorFollowers = tweet.Author.FollowerCount,
            AuthorIsBlueVerified = tweet.Author.IsBlueVerified,
            AuthorIsGoldVerified = tweet.Author.IsVerified,

            // Tweet Information
            TweetId = tweet.Id,
            TweetContent = tweet.Content,
            TweetLink = tweet.Url,
            TweetCreatedAt = tweet.CreatedAt,

            // Token Information
            CoinMintAddress = coinMint,
        };
    }

    /// <summary>
    /// Generates a deterministic GUID from a tweet ID to ensure consistent alert IDs.
    /// This prevents duplicate alerts from the same tweet.
    /// </summary>
    private static Guid GenerateAlertIdFromTweetId(long tweetId)
    {
        // Convert tweet ID to bytes (8 bytes for long)
        var tweetIdBytes = BitConverter.GetBytes(tweetId);

        // Create a 16-byte array for GUID (pad with zeros)
        var guidBytes = new byte[16];
        Array.Copy(tweetIdBytes, 0, guidBytes, 0, 8);

        // Set version to 4 (random UUID) and variant bits according to RFC 4122
        guidBytes[7] = (byte)((guidBytes[7] & 0x0F) | 0x40); // Version 4
        guidBytes[8] = (byte)((guidBytes[8] & 0x3F) | 0x80); // Variant bits

        return new Guid(guidBytes);
    }

    private static AlertSeverity DetermineAlertSeverity(Tweet tweet, AlertType alertType)
    {
        return alertType switch
        {
            AlertType.TwitterLegit when tweet.Author.FollowerCount > 100000 => AlertSeverity.High,
            AlertType.TwitterLegit => AlertSeverity.Medium,
            AlertType.TwitterSpam => AlertSeverity.Low,
            AlertType.TwitterPreLaunch => AlertSeverity.Medium,
            _ => AlertSeverity.Low
        };
    }

    private static string GenerateAlertTitle(Tweet tweet, AlertType alertType)
    {
        return alertType switch
        {
            AlertType.TwitterLegit => $"Legit Tweet from @{tweet.Author.UserName}",
            AlertType.TwitterSpam => $"Potential Spam from @{tweet.Author.UserName}",
            AlertType.TwitterPreLaunch => $"Pre-launch Tweet from @{tweet.Author.UserName}",
            _ => $"Tweet Alert from @{tweet.Author.UserName}"
        };
    }

    private static string GenerateAlertMessage(Tweet tweet, AlertType alertType)
    {
        var truncatedContent = tweet.Content.Length > 100
            ? tweet.Content[..100] + "..."
            : tweet.Content;

        return alertType switch
        {
            AlertType.TwitterLegit => $"Verified account posted about token: {truncatedContent}",
            AlertType.TwitterSpam => $"Potential spam detected: {truncatedContent}",
            AlertType.TwitterPreLaunch => $"Pre-launch mention detected: {truncatedContent}",
            _ => truncatedContent
        };
    }
}
