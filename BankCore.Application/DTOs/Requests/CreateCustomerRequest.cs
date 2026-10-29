namespace BankCore.Application.DTOs.Requests;

public record CreateCustomerRequest(
    string FirstName,
    string LastName,
    string Email,
    DateTime DateOfBirth);