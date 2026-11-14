using BankCore.Domain.Entities;

namespace BankCore.Application.Interfaces.Repositories;

public interface IOutboxRepository
{
    Task AddAsync(OutboxMessage message);

    Task<List<OutboxMessage>> GetPendingMessagesAsync(int batchSize,Guid claimId,TimeSpan claimDuration);

    Task<int> DeleteProcessedMessagesAsync(DateTime olderThan);
    Task<bool> MarkAsProcessedAsync(int messageId, Guid claimId);

    Task<bool> RecordFailureAsync(int messageId,Guid claimId,string error,int maxRetryCount);

    Task<bool> MarkAsFailedAsync(int messageId, Guid claimId);
}
