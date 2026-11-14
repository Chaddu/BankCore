using BankCore.Application.DTOs.Requests;
using BankCore.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankCore.API.Controllers;

[Authorize]
public class BankAccountController(IBankAccountService bankAccountService) : BaseController
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateAccountRequest request)
    {
        var result = await bankAccountService.CreateAsync(request, GetUserId());
        return HandleResult(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await bankAccountService.GetByIdAsync(id, GetUserId());
        return HandleResult(result);
    }

    [HttpGet("number/{accountNumber}")]
    public async Task<IActionResult> GetByAccountNumber(string accountNumber)
    {
        var result = await bankAccountService.GetByAccountNumberAsync(accountNumber, GetUserId());
        return HandleResult(result);
    }

    [HttpGet("my-accounts")]
    public async Task<IActionResult> GetMyAccounts()
    {
        var result = await bankAccountService.GetMyAccountsAsync(GetUserId());
        return HandleResult(result);
    }

    [HttpGet("owner/{ownerId:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetByOwnerId(int ownerId)
    {
        var result = await bankAccountService.GetByOwnerIdAsync(ownerId);
        return HandleResult(result);
    }
    [HttpPost("{id:int}/deposit")]
    public async Task<IActionResult> Deposit(int id, DepositRequest request)
    {
        var result = await bankAccountService.DepositAsync(id,request,GetUserId());

        return HandleResult(result);
    }
}