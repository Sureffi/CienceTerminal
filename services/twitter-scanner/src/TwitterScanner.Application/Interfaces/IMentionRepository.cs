using CienceTerminal.Contracts.Models;

namespace TwitterScanner.Application.Interfaces;

/// <summary>
/// Repository for persisting CA mention records.
/// </summary>
public interface IMentionRepository
{
    Task<bool> AddMentionAsync(CaMentionRecord record, CancellationToken cancellationToken = default);
}
