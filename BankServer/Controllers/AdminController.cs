using BankServer.Controllers.Types;
using BankServer.Helpers;
using BankServer.Models.Roles;
using BankServer.Services;
using BankServer.Services.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankServer.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = UserRoles.Admin)]
[Produces(HttpHeaders.JsonContentHeader)]
public class AdminController : ControllerBase
{
    private readonly AccountService accountService;
    private readonly UserService userService;

    public AdminController(AccountService accountService, UserService userService)
    {
        this.accountService = accountService;
        this.userService = userService;
    }

    [HttpGet]
    [Route("User")]
    public async Task<ActionResult<UserInfo>> GetUserAsync(Guid userId)
        => Ok(await userService.GetUserAsync(userId));

    [HttpGet]
    [Route("AllUsers")]
    public async Task<ActionResult<UserInfo[]>> GetAllUsersAsync()
        => Ok(await userService.GetAllUsersAsync());

    [HttpPatch]
    [Route("AddMoneyToAccount")]
    public async Task<IActionResult> AddMoneyToAccountAsync(Guid accountId, decimal amount)
    {
        await accountService.TryAddMoneyToAccountAsync(accountId, amount);
        return Ok();
    }
}