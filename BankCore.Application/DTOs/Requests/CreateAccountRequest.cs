namespace BankCore.Application.DTOs.Requests;

public record CreateAccountRequest(int OwnerId,string Currency);