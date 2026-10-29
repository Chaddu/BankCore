using BankCore.Domain.Enums;

namespace BankCore.Application.DTOs.Responses;

public record TransferResponse(
    int Id,
    int FromAccountId,
    int ToAccountId,
    decimal Amount,
    string Currency,
    string? Description,
    Guid TransactionId,
    TransferStatus Status,
    DateTime CreatedAt,
    DateTime? UpdatedAt);