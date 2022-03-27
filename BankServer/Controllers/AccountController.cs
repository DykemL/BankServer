using BankServer.Extentions;
using BankServer.Helpers;
using BankServer.Models.DtoModels;
using BankServer.Models.Roles;
using BankServer.Services;
using BankServer.Services.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankServer.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = UserRoles.User)]
[Produces(HttpHeaders.JsonContentHeader)]
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
        var wasAdded = await accountService.TryAddNewEmptyAccountAsync(userId, rubbleCurrency!);
        if (!wasAdded)
        {
            NotFound();
        }
        return Ok();
    }

    [HttpGet]
    [Route("All")]
    public async Task<ActionResult<AccountInfo[]>> GetAllUserAccountsAsync()
    {
        var userId = User.GetId();
        var accounts = await accountService.GetAccounts(userId);
        return Ok(accounts);
    }

    [HttpPatch]
    [Route("TransferMoney")]
    public async Task<IActionResult> TransferMoneyAsync(Guid accountFromId, Guid accountToId, decimal amount)
    {
        var currentUserAccounts = await accountService.GetAccounts(User.GetId());
        if (!currentUserAccounts.Select(x => x.AccountId).Contains(accountFromId))
        {
            return BadRequest("Можно переводить деньги только со своих счетов");
        }

        var transferResult = await accountService.TryTransferMoneyFromToAccount(amount, accountFromId, accountToId);
        if (!transferResult)
        {
            return Problem("Произошла проблема с переводом. Перевод не выполнен");
        }

        return Ok();
    }
}