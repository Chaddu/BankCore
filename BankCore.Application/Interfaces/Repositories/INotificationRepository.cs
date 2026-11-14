
using BankCore.Domain.Entities;

namespace BankCore.Application.Interfaces.Repositories;

public interface INotificationRepository
{
    Task AddAsync(Notification notification);

    Task<Notification?> GetByIdAsync(int id);

    Task<List<Notification>> GetByCustomerIdAsync(int customerId);

    Task<int> CountUnreadAsync(int customerId);
    Task<bool> ExistsByTransactionIdAsync(Guid transactionId,int customerId);
}
