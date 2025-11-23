using CienceTerminal.Contracts.Events;
using CienceTerminal.Contracts.Models;
using MediatR;
using TwitterScanner.Application.Extensions;
using TwitterScanner.Application.Interfaces;
using TwitterScanner.Application.Messaging.Commands;
using TwitterScanner.Domain.Events;

namespace TwitterScanner.Application.Messaging.Notifications;

public class CaMentionReceivedNotificationHandler : INotificationHandler<CaMentionReceivedNotification>
{
    private readonly IMentionRepository _mentionRepository;
    private readonly IMediator _mediator;

    public CaMentionReceivedNotificationHandler(
        IMentionRepository mentionRepository,
        IMediator mediator)
    {
        _mentionRepository = mentionRepository;
        _mediator = mediator;
    }

    public async Task Handle(CaMentionReceivedNotification notification, CancellationToken cancellationToken)
    {
        var mention = notification.CaMention;

        CaMentionRecord record = mention.ToCaMentionRecord();

        // Add mention record to database
        bool wasAdded = await _mentionRepository.AddMentionAsync(record, cancellationToken);

        // Only publish SNS event if the mention was successfully added to the database
        if (!wasAdded)
        {
            return;
        }

        // TODO: Better mapping
        var detectedEvent = new CaMentionDetectedEvent
        {
            Id = record.Id,
            CoinMintAddress = record.CoinMintAddress,
            TweetId = record.TweetId,
            AuthorId = record.AuthorId,
            Username = record.Username,
            ProfilePicture = record.ProfilePicture,
            TweetUrl = record.TweetUrl,
            TweetContent = record.TweetContent,
            Followers = record.Followers,
            IsVerified = record.IsVerified,
            Timestamp = record.Timestamp,
            IsOriginalPost = record.IsOriginalPost,
            IsReply = record.IsReply,
            IsQuote = record.IsQuote,
            IsRetweet = record.IsRetweet
        };

        await _mediator.Send(new PublishCaMentionDetectedCommand(detectedEvent), cancellationToken);
    }
}
