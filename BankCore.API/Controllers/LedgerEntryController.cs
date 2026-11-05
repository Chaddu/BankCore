using BankCore.Application.DTOs.Requests;
using BankCore.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankCore.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LedgerEntryController(ILedgerEntryService ledgerEntryService) : BaseController
{
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await ledgerEntryService.GetByIdAsync(id);
        return HandleResult(result);
    }

    [HttpGet("account/{accountId:int}")]
    public async Task<IActionResult> GetByAccountId(int accountId, [FromQuery] PaginationRequest request)
    {
        var result = await ledgerEntryService.GetByAccountIdAsync(accountId,request);

        return HandleResult(result);
    }
}