using MediatR;
using TwitterScanner.Domain.Entities;
using TwitterScanner.Domain.Groq.Prompts;

namespace TwitterScanner.Application.Messaging.Requests;

/// <summary>
/// Mediator request to classify tweet with Groq
/// </summary>
/// <param name="Tweet"></param>
public record ClassifyTweetRequest(Tweet Tweet) : IRequest<TweetClassifierResult>;
