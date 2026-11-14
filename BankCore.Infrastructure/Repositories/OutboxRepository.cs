using BankCore.Application.Interfaces.Repositories;
using BankCore.Domain.Entities;
using BankCore.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BankCore.Infrastructure.Repositories;

public class OutboxRepository(AppDbContext context) : IOutboxRepository
{
    public async Task AddAsync(OutboxMessage message)
    {
        await context.OutboxMessages.AddAsync(message);
    }

    public async Task<int> DeleteProcessedMessagesAsync(DateTime olderThan)
    {
        return await context.OutboxMessages.Where(m => m.ProcessedAt != null && m.ProcessedAt < olderThan).ExecuteDeleteAsync();
    }

    public async Task<List<OutboxMessage>> GetPendingMessagesAsync(int batchSize, Guid claimId, TimeSpan claimDuration)
    {
        var now = DateTime.UtcNow;
        var claimedUntil = now.Add(claimDuration);

        await context.Database.ExecuteSqlInterpolatedAsync($"""
        ;WITH MessagesToClaim AS
        (
            SELECT TOP ({batchSize}) *
            FROM OutboxMessages WITH (UPDLOCK, READPAST, ROWLOCK)
            WHERE ProcessedAt IS NULL
        AND FailedAt IS NULL
        AND (ClaimedUntil IS NULL OR ClaimedUntil < {now})
            ORDER BY OccurredAt
        )
        UPDATE MessagesToClaim
        SET ClaimId = {claimId},
            ClaimedUntil = {claimedUntil},
            UpdatedAt = {now};
        """);

        context.ChangeTracker.Clear();

        return await context.OutboxMessages
            .Where(m => m.ClaimId == claimId)
            .OrderBy(m => m.OccurredAt)
            .ToListAsync();
    }
    public async Task<bool> MarkAsProcessedAsync(int messageId, Guid claimId)
    {
        var now = DateTime.UtcNow;

        var updatedRows = await context.OutboxMessages
            .Where(m =>
                m.Id == messageId &&
                m.ClaimId == claimId &&
                m.ClaimedUntil > now &&
                m.ProcessedAt == null &&
                m.FailedAt == null)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(m => m.ProcessedAt, now)
                .SetProperty(m => m.LastError, (string?)null)
                .SetProperty(m => m.ClaimId, (Guid?)null)
                .SetProperty(m => m.ClaimedUntil, (DateTime?)null)
                .SetProperty(m => m.UpdatedAt, now));

        return updatedRows > 0;
    }
    public async Task<bool> RecordFailureAsync(
    int messageId,
    Guid claimId,
    string error,
    int maxRetryCount)
    {
        var now = DateTime.UtcNow;

        var updatedRows = await context.OutboxMessages
            .Where(m =>
                m.Id == messageId &&
                m.ClaimId == claimId &&
                m.ClaimedUntil > now &&
                m.ProcessedAt == null &&
                m.FailedAt == null)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(m => m.RetryCount, m => m.RetryCount + 1)
                .SetProperty(m => m.LastError, error)
                .SetProperty(
                    m => m.FailedAt,
                    m => m.RetryCount + 1 >= maxRetryCount
                        ? now
                        : (DateTime?)null)
                .SetProperty(m => m.ClaimId, (Guid?)null)
                .SetProperty(m => m.ClaimedUntil, (DateTime?)null)
                .SetProperty(m => m.UpdatedAt, now));

        return updatedRows > 0;
    }

    public async Task<bool> MarkAsFailedAsync(int messageId, Guid claimId)
    {
        var now = DateTime.UtcNow;

        var updatedRows = await context.OutboxMessages
            .Where(m =>
                m.Id == messageId &&
                m.ClaimId == claimId &&
                m.ClaimedUntil > now &&
                m.ProcessedAt == null &&
                m.FailedAt == null)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(m => m.FailedAt, now)
                .SetProperty(m => m.ClaimId, (Guid?)null)
                .SetProperty(m => m.ClaimedUntil, (DateTime?)null)
                .SetProperty(m => m.UpdatedAt, now));

        return updatedRows > 0;
    }
}
