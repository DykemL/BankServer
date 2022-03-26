using BankServer.Models.Roles;
using BankServer.Services;
using BankServer.Services.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankServer.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = UserRoles.Admin)]
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
    [Route("GetUser")]
    public async Task<IActionResult> GetUserAsync(Guid userId)
        => Ok(await userService.GetUser(userId));

    [HttpGet]
    [Route("GetAllUsers")]
    public async Task<IActionResult> GetAllUsersAsync()
        => Ok(await userService.GetAllUsers());

    [HttpGet]
    [Route("AddMoneyToAccount")]
    public async Task<IActionResult> AddMoneyToAccountAsync(Guid accountId, decimal amount)
    {
        await accountService.AddMoneyToAccount(accountId, amount);
        return Ok();
    }
}