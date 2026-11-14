using BankCore.Application.Common;

namespace BankCore.Application.Interfaces.Services;

public interface IDeadLetterQueueService
{
    Task<Result> ReplayNextMessageAsync(CancellationToken cancellationToken = default);
}
