using IdentityGrpc.DTOs;
using IdentityGrpc.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IdentityGrpc.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountsController(AccountService accountService) : ControllerBase
{
    private readonly AccountService _accountService = accountService;

    [HttpPost("Create")]
    public async Task<IActionResult> Create(CreateAccountRequestModel request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var response = await _accountService.CreateAccount(request);
        return response.Success
            ? Ok(response) 
            : Problem("Something went wrong");
    }


    [HttpPost("Validate")]
    public async Task<IActionResult> Validate(ValidateCredentialsRequestModel request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var response = await _accountService.ValidateCredentials(request);
        return response.Success
            ? Ok(response)
            : Problem("Something went wrong");
    }
}
