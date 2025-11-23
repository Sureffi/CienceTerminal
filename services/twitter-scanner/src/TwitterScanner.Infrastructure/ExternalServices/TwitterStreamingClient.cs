using System.Globalization;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using TwitterScanner.Application.Interfaces;
using TwitterScanner.Domain.Entities;

namespace TwitterScanner.Infrastructure.ExternalServices;

public class TwitterStreamingClient : ITwitterStreamingClient
{

    private readonly Uri _uri;
    private readonly string _apiKey;

    private ClientWebSocket _websocket;
    private CancellationTokenSource _messageHandlerCts = new();

    public TwitterStreamingClient(IConfiguration configuration)
    {
        _apiKey = configuration["ApiKeys:Twitter"];
        _uri = new Uri(configuration["Endpoints:Twitter"]);
    }

    /// <summary>
    /// Connects the twitter api and starts processing tweets
    /// This method can be awaited
    /// </summary>
    /// <param name="onMessage"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task ConnectAsync(Action<Tweet> onMessage, CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                // Cancel and create new message handler cts
                _messageHandlerCts.Cancel();
                _messageHandlerCts.Dispose();
                _messageHandlerCts = new CancellationTokenSource();

                // Create new websocket client
                _websocket?.Dispose();
                _websocket = new ClientWebSocket();

                // Set api key header and try to connect
                _websocket.Options.SetRequestHeader("x-api-key", _apiKey);
                await _websocket.ConnectAsync(_uri, cancellationToken);

                // Fire and forget message handler
                _ = HandleMessages(onMessage, _messageHandlerCts.Token, cancellationToken);

                // Succesfully connected
                Console.WriteLine("Twitter client connected");
                break;
            }
            catch (Exception ex)
            {
                // Connection error
                // await to retry connection
                Console.WriteLine($"Error connecting to twitter: {ex}");
                Console.WriteLine("Waiting to reconnect");
                await Task.Delay(21000, cancellationToken);
            }
        }
    }

    private async Task HandleMessages(Action<Tweet> onMessage, CancellationToken handlerCt, CancellationToken stoppingToken)
    {
        try
        {
            byte[] receiveBuffer = new byte[4096];
            StringBuilder messageBuilder = new StringBuilder();
            while (!handlerCt.IsCancellationRequested && !stoppingToken.IsCancellationRequested && _websocket.State == WebSocketState.Open)
            {
                WebSocketReceiveResult result = await _websocket.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), handlerCt);
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    string fragment = Encoding.UTF8.GetString(receiveBuffer, 0, result.Count);
                    messageBuilder.Append(fragment);

                    if (result.EndOfMessage)
                    {
                        string completeMessage = messageBuilder.ToString();
                        ProcessMessage(completeMessage, onMessage);
                        messageBuilder.Clear();
                    }
                }

            }
        }
        catch (OperationCanceledException)
        {
            return;
        }
        catch (ObjectDisposedException)
        {
            return;
        }
        catch (WebSocketException)
        {
            // Websocket exception meaning websocket went offline
            // If program was not stopped
            if (!stoppingToken.IsCancellationRequested)
            {
                // Reconnect socket
                _ = Task.Run(() => ConnectAsync(onMessage, stoppingToken), stoppingToken);
            }
        }

    }

    private void ProcessMessage(string json, Action<Tweet> onMessage)
    {
        try
        {
            using var document = JsonDocument.Parse(json);
            var root = document.RootElement;

            if (root.TryGetProperty("event_type", out var eventType))
            {
                if (eventType.GetString() == "tweet" && root.TryGetProperty("tweets", out var tweetsArray))
                {
                    foreach (var tweetJson in tweetsArray.EnumerateArray())
                    {
                        var tweet = ParseTweet(tweetJson);
                        if (tweet != null)
                        {
                            onMessage(tweet);
                        }
                    }
                }
                else if (eventType.GetString() == "ping")
                {
                    Console.WriteLine("Ping received");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing message: {ex.Message}");
        }
    }

    private Tweet? ParseTweet(JsonElement tweetJson)
    {
        try
        {
            var tweet = new Tweet
            {
                Id = SafeParseLong(tweetJson.TryGetProperty("id", out var idProp) ? idProp.GetString() : ""),
                Url = tweetJson.TryGetProperty("url", out var urlProp) ? urlProp.GetString() : "",
                Content = tweetJson.TryGetProperty("text", out var textProp) ? textProp.GetString() : "",
                ReTweetCount = SafeGetInt32(tweetJson, "retweetCount"),
                ReplyCount = SafeGetInt32(tweetJson, "replyCount"),
                LikeCount = SafeGetInt32(tweetJson, "likeCount"),
                QuoteCount = SafeGetInt32(tweetJson, "quoteCount"),
                ViewCount = SafeGetInt32(tweetJson, "viewCount"),
                BookmarkCount = SafeGetInt32(tweetJson, "bookmarkCount"),
                CreatedAt = tweetJson.TryGetProperty("createdAt", out var createdAtProp) ?
                    ParseTwitterDateTime(createdAtProp.GetString()) : DateTime.MinValue,
                Lang = tweetJson.TryGetProperty("lang", out var langProp) ? langProp.GetString() : "",
                IsReply = tweetJson.TryGetProperty("isReply", out var isReplyProp) ? isReplyProp.GetBoolean() : false,
                ConversationId = tweetJson.TryGetProperty("conversationId", out var convIdProp) ?
                    SafeParseLong(convIdProp.GetString()) : 0
            };

            // Parse inReplyToId if tweet is a reply
            if (tweetJson.TryGetProperty("inReplyToId", out var inReplyToId) && inReplyToId.ValueKind != JsonValueKind.Null)
                tweet.InReplyToId = SafeParseLong(inReplyToId.GetString());

            // Parse inReplyToUserId if tweet is a reply
            if (tweetJson.TryGetProperty("inReplyToUserId", out var inReplyToUserId) && inReplyToUserId.ValueKind != JsonValueKind.Null)
                tweet.InReplyToUserId = SafeParseLong(inReplyToUserId.GetString());

            tweet.InReplyToUsername = tweetJson.TryGetProperty("inReplyToUsername", out var inReplyToUsernameProp) ?
                (inReplyToUsernameProp.GetString() ?? "") : "";

            // Parse quoted tweet if tweet is a quote
            if (tweetJson.TryGetProperty("quoted_tweet", out var quotedTweetJson) && quotedTweetJson.ValueKind != JsonValueKind.Null)
            {
                try
                {
                    tweet.QuotedTweet = ParseTweet(quotedTweetJson);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error parsing quoted tweet: {ex.Message}");
                }
            }

            // Parse retweeted tweet if tweet is a retweet
            if (tweetJson.TryGetProperty("retweeted_tweet", out var retweetedTweetJson) && retweetedTweetJson.ValueKind != JsonValueKind.Null)
            {
                try
                {
                    tweet.RetweetedTweet = ParseTweet(retweetedTweetJson);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error parsing retweeted tweet: {ex.Message}");
                    Console.WriteLine($"Retweeted tweet JSON structure: {retweetedTweetJson}");
                    // Set to null to prevent further issues
                    tweet.RetweetedTweet = null;
                }
            }

            tweet.IsQuote = tweet.QuotedTweet != null;
            tweet.IsRetweet = tweet.RetweetedTweet != null;
            tweet.IsOriginalPost = !tweet.IsReply && !tweet.IsQuote && !tweet.IsRetweet;

            if (tweetJson.TryGetProperty("ruleId", out var ruleId))
                tweet.RuleId = ruleId.GetString() ?? "";

            if (tweetJson.TryGetProperty("ruleTag", out var ruleTag))
                tweet.RuleTag = ruleTag.GetString() ?? "";

            if (tweetJson.TryGetProperty("author", out var authorJson))
            {
                tweet.Author = new Author
                {
                    Id = SafeParseLong(authorJson.TryGetProperty("id", out var authorIdProp) ? authorIdProp.GetString() : ""),
                    UserName = authorJson.TryGetProperty("userName", out var userNameProp) ? userNameProp.GetString() : "",
                    Name = authorJson.TryGetProperty("name", out var nameProp) ? nameProp.GetString() : "",
                    Url = authorJson.TryGetProperty("url", out var authorUrlProp) ? authorUrlProp.GetString() : "",
                    ProfilePicture = authorJson.TryGetProperty("profilePicture", out var profilePicProp) ? profilePicProp.GetString() : "",
                    IsVerified = authorJson.TryGetProperty("isVerified", out var isVerifiedProp) ? isVerifiedProp.GetBoolean() : false,
                    IsBlueVerified = authorJson.TryGetProperty("isBlueVerified", out var isBlueVerifiedProp) ? isBlueVerifiedProp.GetBoolean() : false,
                    FollowerCount = SafeGetInt32(authorJson, "followers") + SafeGetInt32(authorJson, "followersCount"),
                    FollowingCount = SafeGetInt32(authorJson, "following") + SafeGetInt32(authorJson, "followingCount"),
                    CreatedAt = authorJson.TryGetProperty("createdAt", out var authorCreatedAtProp) ?
                        ParseTwitterDateTime(authorCreatedAtProp.GetString()) : DateTime.MinValue,
                    IsAutomated = authorJson.TryGetProperty("isAutomated", out var isAutomatedProp) ? isAutomatedProp.GetBoolean() : false,
                    FavouritesCount = SafeGetInt32(authorJson, "favouritesCount") + SafeGetInt32(authorJson, "likeCount")
                };
            }
            else
            {
                // Create a default author if missing
                tweet.Author = new Author
                {
                    Id = 0,
                    UserName = "Unknown",
                    Name = "Unknown User",
                    Url = "",
                    ProfilePicture = "",
                    IsVerified = false,
                    IsBlueVerified = false,
                    FollowerCount = 0,
                    FollowingCount = 0,
                    CreatedAt = DateTime.MinValue,
                    IsAutomated = false,
                    FavouritesCount = 0
                };
            }

            return tweet;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error parsing tweet: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");

            // Return null to indicate parsing failure instead of throwing
            return null;
        }

    }

    /// <summary>
    /// Helper method to parse twitter time into DateTime
    /// </summary>
    /// <param name="dateTimeString"></param>
    /// <returns></returns>
    private static DateTime ParseTwitterDateTime(string dateTimeString)
    {
        // Handle empty or null datetime strings
        if (string.IsNullOrWhiteSpace(dateTimeString))
        {
            return DateTime.MinValue;
        }

        try
        {
            // Twitter format: "Mon Sep 01 17:56:01 +0000 2025"
            return DateTime.ParseExact(dateTimeString, "ddd MMM dd HH:mm:ss zzz yyyy", CultureInfo.InvariantCulture);
        }
        catch (FormatException)
        {
            Console.WriteLine($"Failed to parse Twitter datetime: '{dateTimeString}'");
            return DateTime.MinValue;
        }
    }

    /// <summary>
    /// Helper method to safely parse long values from strings, handling empty/null values
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    private static long SafeParseLong(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return 0;
        }

        if (long.TryParse(value, out long result))
        {
            return result;
        }

        Console.WriteLine($"Failed to parse long value: '{value}'");
        return 0;
    }

    /// <summary>
    /// Helper method to safely get integer values from JsonElement, handling missing/null properties
    /// </summary>
    /// <param name="element"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    private static int SafeGetInt32(JsonElement element, string propertyName)
    {
        if (element.TryGetProperty(propertyName, out var property))
        {
            if (property.ValueKind == JsonValueKind.Number)
            {
                return property.GetInt32();
            }
            if (property.ValueKind == JsonValueKind.String)
            {
                var stringValue = property.GetString();
                if (int.TryParse(stringValue, out int result))
                {
                    return result;
                }
                // Only log if string value is not empty (empty strings are expected)
                if (!string.IsNullOrEmpty(stringValue))
                {
                    Console.WriteLine($"Failed to parse string '{stringValue}' as int32 for property: '{propertyName}'");
                }
                return 0;
            }
            if (property.ValueKind == JsonValueKind.Null)
            {
                return 0;
            }
            Console.WriteLine($"Unexpected value kind '{property.ValueKind}' for property: '{propertyName}', value: '{property}'");
            return 0;
        }

        // Don't log missing properties as they might be optional
        return 0;
    }
}
