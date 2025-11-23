using MediatR;
using Solnet.Rpc.Models;

namespace TwitterScanner.Application.Messaging.Requests;

/// <summary>
/// Request parsed token mint data from solana RPC
/// </summary>
/// <param name="Mint"></param>
public record GetParsedTokenMintDataRequest(string Mint) : IRequest<ParsedTokenMintData?>;
