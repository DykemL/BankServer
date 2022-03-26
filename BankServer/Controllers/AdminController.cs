using BankServer.Models.Roles;
using BankServer.Services;
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
    public async Task<IActionResult> GetUser(Guid userId)
        => Ok(await userService.GetUser(userId));

    [HttpGet]
    [Route("GetAllUsers")]
    public async Task<IActionResult> GetAllUsers()
        => Ok(await userService.GetAllUsers());

    [HttpGet]
    [Route("TopUpAccount")]
    public async Task<IActionResult> TopUpAccountAsync(Guid accountId, decimal amount)
    {
        await accountService.TopUpAccount(accountId, amount);
        return Ok();
    }
}