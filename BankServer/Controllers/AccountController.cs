using BankServer.Extentions;
using BankServer.Models.Roles;
using BankServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankServer.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = UserRoles.User)]
public class AccountController : ControllerBase
{
    private readonly AccountService accountService;

    public AccountController(AccountService accountService)
        => this.accountService = accountService;

    [HttpPut]
    public async Task<IActionResult> AddNewEmptyAccountAsync()
    {
        var userId = User.GetId();
        await accountService.AddNewEmptyAccountAsync(userId).ConfigureAwait(false);
        return Ok();
    }

    [HttpGet]
    [Route("GetAll")]
    public async Task<IActionResult> GetAll()
    {
        var userId = User.GetId();
        var accounts = await accountService.GetAccounts(userId).ConfigureAwait(false);
        return Ok(accounts);
    }
}