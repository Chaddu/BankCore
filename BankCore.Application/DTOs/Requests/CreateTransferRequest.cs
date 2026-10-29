namespace BankCore.Application.DTOs.Requests;

public record CreateTransferRequest(
    int FromAccountId,
    int ToAccountId,
    decimal Amount,
    string Currency,
    string? Description);