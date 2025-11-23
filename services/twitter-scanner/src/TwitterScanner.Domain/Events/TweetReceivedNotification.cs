using TwitterScanner.Domain.Entities;
using MediatR;

namespace TwitterScanner.Domain.Events;

public record TweetReceivedNotification(Tweet Tweet) : INotification;
