namespace BankCore.Application.DTOs.Responses;

public record CustomerResponse(
    int Id,
    string FirstName,
    string LastName,
    string Email,
    DateTime DateOfBirth,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
