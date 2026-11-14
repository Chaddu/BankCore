using BankCore.Application.Interfaces.Services;
using BankCore.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankCore.API.Controllers;

[Authorize(Roles = "Admin")]
public class DeadLetterQueueController(
    IDeadLetterQueueService deadLetterQueueService) : BaseController
{
    [HttpPost("replay-next")]
    public async Task<IActionResult> ReplayNext(CancellationToken cancellationToken)
    {
        var result = await deadLetterQueueService.ReplayNextMessageAsync(cancellationToken);

        return HandleResult(result);
    }
}