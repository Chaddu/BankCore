using BankCore.Application.Interfaces.Repositories;
using BankCore.Domain.Entities;
using BankCore.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BankCore.Infrastructure.Repositories;

public class NotificationRepository(AppDbContext context) : INotificationRepository
{
    public async Task AddAsync(Notification notification)
    {
        await context.Notifications.AddAsync(notification);
    }

    public async Task<Notification?> GetByIdAsync(int id)
    {
        return await context.Notifications.FirstOrDefaultAsync(n => n.Id == id);
    }

    public async Task<List<Notification>> GetByCustomerIdAsync(int customerId)
    {
        return await context.Notifications.Where(n => n.CustomerId == customerId).OrderByDescending(n => n.CreatedAt).ToListAsync();
    }

    public async Task<int> CountUnreadAsync(int customerId)
    {
        return await context.Notifications.CountAsync(n => n.CustomerId == customerId && !n.IsRead);
    }

    public async Task<bool> ExistsByTransactionIdAsync(Guid transactionId, int customerId)
    {
        return await context.Notifications.AnyAsync(n => n.TransactionId == transactionId && n.CustomerId == customerId);
    }
}
