using BankCore.Application.DTOs.Requests;
using BankCore.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankCore.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CustomerController(ICustomerService customerService) : BaseController
{
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await customerService.GetByIdAsync(id);
        return HandleResult(result);
    }

    [HttpGet("email/{email}")]
    public async Task<IActionResult> GetByEmail(string email)
    {
        var result = await customerService.GetByEmailAsync(email);
        return HandleResult(result);
    }

    [HttpPut]
    public async Task<IActionResult> Update(UpdateCustomerRequest request)
    {
        var result = await customerService.UpdateAsync(GetUserId(), request);

        return HandleResult(result);
    }
}