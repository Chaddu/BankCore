namespace BankCore.Application.DTOs.Requests;

public record PaginationRequest(
    int Page = 1,
    int PageSize = 10);