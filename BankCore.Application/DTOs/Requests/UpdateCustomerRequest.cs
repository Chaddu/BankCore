namespace BankCore.Application.DTOs.Requests;

public record UpdateCustomerRequest(
    string FirstName,
    string LastName,
    string Email);