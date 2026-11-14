using BankCore.Application.DTOs.Requests;
using BankCore.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankCore.API.Controllers;

[Authorize]
public class TransferController(ITransferService transferService) : BaseController
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateTransferRequest request,[FromHeader(Name = "Idempotency-Key")] string idempotencyKey)
    {
        var result = await transferService.CreateAsync(request,idempotencyKey, GetUserId());

        return HandleResult(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await transferService.GetByIdAsync(id, GetUserId());
        return HandleResult(result);
    }
}
