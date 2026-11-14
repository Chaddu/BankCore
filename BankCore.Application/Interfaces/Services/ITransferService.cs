using BankCore.Application.Common;
using BankCore.Application.DTOs.Requests;
using BankCore.Application.DTOs.Responses;

namespace BankCore.Application.Interfaces.Services;
public interface ITransferService
{
    Task<Result<TransferResponse>> CreateAsync(CreateTransferRequest request,string idempotencyKey, int currentUserId);

    Task<Result<TransferResponse>> GetByIdAsync(int id, int currentUserId);

}
