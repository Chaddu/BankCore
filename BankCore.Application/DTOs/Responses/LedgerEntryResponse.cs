using BankCore.Domain.Enums;

namespace BankCore.Application.DTOs.Responses;

public record LedgerEntryResponse(
    int Id,
    int BankAccountId,
    decimal Amount,
    TransactionType Type,
    string Description,
    Guid TransactionId,
    DateTime CreatedAt);