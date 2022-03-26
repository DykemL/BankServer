using BankServer.Extentions;
using BankServer.Models.Roles;
using BankServer.Providers;
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
    private readonly CurrencyService currencyService;

    public AccountController(AccountService accountService, CurrencyService currencyService)
    {
        this.accountService = accountService;
        this.currencyService = currencyService;
    }

    [HttpPut]
    public async Task<IActionResult> AddNewEmptyAccountAsync(string currencyName = CurrencyHelper.Rubble)
    {
        var userId = User.GetId();
        var rubbleCurrency = await currencyService.GetCurrency(currencyName);
        await accountService.AddNewEmptyAccountAsync(userId, rubbleCurrency!);
        return Ok();
    }

    [HttpGet]
    [Route("GetAll")]
    public async Task<IActionResult> GetAllAsync()
    {
        var userId = User.GetId();
        var accounts = await accountService.GetAccounts(userId);
        return Ok(accounts);
    }
}