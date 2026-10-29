using BankCore.Application.Interfaces.Repositories;
using BankCore.Domain.Entities;
using BankCore.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BankCore.Infrastructure.Repositories;

public class TransferRepository(AppDbContext context) : ITransferRepository
{
    public async Task<Transfer?> GetByIdAsync(int id)
    {
        return await context.Transfers.FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<Transfer?> GetByTransactionIdAsync(Guid transactionId)
    {
        return await context.Transfers.FirstOrDefaultAsync(t => t.TransactionId == transactionId);
    }

    public async Task AddAsync(Transfer transfer)
    {
        await context.Transfers.AddAsync(transfer);
    }
}
