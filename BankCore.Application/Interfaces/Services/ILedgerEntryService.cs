using BankCore.Application.Common;
using BankCore.Application.DTOs.Requests;
using BankCore.Application.DTOs.Responses;

namespace BankCore.Application.Interfaces.Services;
public interface ILedgerEntryService
{
    Task<Result<LedgerEntryResponse>> GetByIdAsync(int id);

    Task<Result<PagedResponse<LedgerEntryResponse>>> GetByAccountIdAsync(int accountId,PaginationRequest request);
}
