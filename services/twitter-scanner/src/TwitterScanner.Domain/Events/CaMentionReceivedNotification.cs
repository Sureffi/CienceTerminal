using TwitterScanner.Domain.Entities;
using MediatR;

namespace TwitterScanner.Domain.Events;

public record CaMentionReceivedNotification(CaMention CaMention) : INotification;