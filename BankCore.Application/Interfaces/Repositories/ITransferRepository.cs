using BankCore.Domain.Entities;

namespace BankCore.Application.Interfaces.Repositories;
public interface ITransferRepository
{
    Task<Transfer?> GetByIdAsync(int id);

    Task<Transfer?> GetByTransactionIdAsync(Guid transactionId);

    Task AddAsync(Transfer transfer);
}
