using BankCore.Application.DTOs.Requests;
using BankCore.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankCore.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BankAccountController(IBankAccountService bankAccountService) : BaseController
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateAccountRequest request)
    {
        var result = await bankAccountService.CreateAsync(request);
        return HandleResult(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await bankAccountService.GetByIdAsync(id);
        return HandleResult(result);
    }

    [HttpGet("number/{accountNumber}")]
    public async Task<IActionResult> GetByAccountNumber(string accountNumber)
    {
        var result = await bankAccountService.GetByAccountNumberAsync(accountNumber);
        return HandleResult(result);
    }

    [HttpGet("my-accounts")]
    public async Task<IActionResult> GetMyAccounts()
    {
        var result = await bankAccountService.GetByOwnerIdAsync(GetUserId());
        return HandleResult(result);
    }
}