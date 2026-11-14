using AutoMapper;
using BankCore.Application.Common;
using BankCore.Application.DTOs.Requests;
using BankCore.Application.DTOs.Responses;
using BankCore.Application.Interfaces;
using BankCore.Application.Interfaces.Services;

namespace BankCore.Application.Services;

public class LedgerEntryService(IUnitOfWork unitOfWork, IMapper mapper) : ILedgerEntryService
{
    public async Task<Result<PagedResponse<LedgerEntryResponse>>> GetByAccountIdAsync(int accountId, PaginationRequest request, int currentUserId)
    {
        var customer = await unitOfWork.Customers.GetByUserIdAsync(currentUserId);

        if (customer is null)
            return Result<PagedResponse<LedgerEntryResponse>>.Failure("Customer not found.", ErrorType.NotFound);

        var account = await unitOfWork.BankAccounts.GetByIdAsync(accountId);

        if(account is null || account.OwnerId != customer.Id)
            return Result<PagedResponse<LedgerEntryResponse>>.Failure("Bank account not found.", ErrorType.NotFound);

        var pagination = PaginationHelper.Validate(request);

        var entries = await unitOfWork.LedgerEntries.GetByAccountIdAsync(accountId,pagination.Page,pagination.PageSize);
        
        var totalCount = await unitOfWork.LedgerEntries.CountByAccountIdAsync(accountId);

        var response = new PagedResponse<LedgerEntryResponse>(
           mapper.Map<List<LedgerEntryResponse>>(entries),
           pagination.Page,
           pagination.PageSize,
           totalCount,
           PaginationHelper.CalculatePages(totalCount,pagination.PageSize));

        return Result<PagedResponse<LedgerEntryResponse>>.Success(response);
    }

    public async Task<Result<LedgerEntryResponse>> GetByIdAsync(int id, int currentUserId)
    {
        var customer = await unitOfWork.Customers.GetByUserIdAsync(currentUserId);

        if (customer is null)
            return Result<LedgerEntryResponse>.Failure("Customer not found.", ErrorType.NotFound);

        var ledgeEntry = await unitOfWork.LedgerEntries.GetByIdAsync(id);
        
        if (ledgeEntry is null)
            return Result<LedgerEntryResponse>.Failure("Ledger entry not found.", ErrorType.NotFound);

        var account = await unitOfWork.BankAccounts.GetByIdAsync(ledgeEntry.BankAccountId);

        if (account is null || account.OwnerId != customer.Id)
            return Result<LedgerEntryResponse>.Failure("Ledger entry not found.", ErrorType.NotFound);

        return Result<LedgerEntryResponse>.Success(mapper.Map<LedgerEntryResponse>(ledgeEntry));
    }
}
