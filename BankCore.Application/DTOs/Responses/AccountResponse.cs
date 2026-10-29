using BankCore.Domain.Enums;

namespace BankCore.Application.DTOs.Responses;

public record AccountResponse(
    int Id,
    string AccountNumber,
    int OwnerId,
    decimal Balance,
    string Currency,
    AccountStatus Status,
    DateTime CreatedAt,
    DateTime? UpdatedAt);