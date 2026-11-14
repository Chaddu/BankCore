using BankCore.Application.Common;
using BankCore.Application.DTOs.Requests;
using BankCore.Application.DTOs.Responses;

namespace BankCore.Application.Interfaces.Services;
public interface ILedgerEntryService
{
    Task<Result<LedgerEntryResponse>> GetByIdAsync(int id, int currentUserId);

    Task<Result<PagedResponse<LedgerEntryResponse>>> GetByAccountIdAsync(int accountId,PaginationRequest request, int currentUserId);
}
