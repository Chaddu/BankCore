using BankCore.Domain.Exceptions;

namespace BankCore.Domain.Entities;

public class OutboxMessage : BaseEntity
{
    public Guid EventId { get; private set; }
    public string Type { get; private set; } = string.Empty;
    public string Content { get; private set; } = string.Empty;
    public DateTime OccurredAt { get; private set; }
    public DateTime? ProcessedAt { get; private set; }
    public int RetryCount { get; private set; }
    public string? LastError { get; private set; }
    public Guid? ClaimId { get; private set; }
    public DateTime? ClaimedUntil { get; private set; }
    public DateTime? FailedAt { get; private set; }

    private OutboxMessage() { }

    public OutboxMessage(string type, string content)
    {
        if (string.IsNullOrWhiteSpace(type))
            throw new DomainException("Event type is required.");

        if (string.IsNullOrWhiteSpace(content))
            throw new DomainException("Event content is required.");

        EventId = Guid.NewGuid();
        Type = type.Trim();
        Content = content;
        OccurredAt = DateTime.UtcNow;
    }

    //Not used for now

    //public void MarkAsProcessed()
    //{
    //    if (ProcessedAt is not null || FailedAt is not null)
    //        return;

    //    ProcessedAt = DateTime.UtcNow;
    //    MarkAsUpdated();
    //}
    //public void RecordFailure(string error)
    //{
    //    RetryCount++;
    //    LastError = error;
    //    MarkAsUpdated();
    //}
    //public void ClearError()
    //{
    //    LastError = null;
    //}

    //public void Claim(Guid claimId, TimeSpan duration)
    //{
    //    if (claimId == Guid.Empty)
    //        throw new DomainException("Claim ID is required.");

    //    if (duration <= TimeSpan.Zero)
    //        throw new DomainException("Claim duration must be positive.");

    //    ClaimId = claimId;
    //    ClaimedUntil = DateTime.UtcNow.Add(duration);

    //    MarkAsUpdated();
    //}

    //public void ReleaseClaim()
    //{
    //    ClaimId = null;
    //    ClaimedUntil = null;

    //    MarkAsUpdated();
    //}
    //public void MarkAsFailed()
    //{
    //    if (ProcessedAt is not null || FailedAt is not null)
    //        return;

    //    FailedAt = DateTime.UtcNow;
    //    ReleaseClaim();
    //    MarkAsUpdated();
    //}
}
