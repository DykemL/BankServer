using BankServer.Extentions;
using BankServer.Helpers;
using BankServer.Models.DtoModels;
using BankServer.Models.Roles;
using BankServer.Services;
using BankServer.Services.Accounts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankServer.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = UserRoles.User)]
[Produces(HttpHeaders.JsonContentHeader)]
public class AccountController : ControllerBase
{
    private const int NotEnoughMoneyCode = 418;

    private readonly AccountService accountService;
    private readonly CurrencyService currencyService;

    public AccountController(AccountService accountService, CurrencyService currencyService)
    {
        this.accountService = accountService;
        this.currencyService = currencyService;
    }

    [HttpPut]
    public async Task<IActionResult> AddNewEmptyAccountAsync([FromBody] AddNewEmptyAccountDto addNewEmptyAccountDto)
    {
        var userId = User.GetId();
        var rubbleCurrency = await currencyService.GetCurrency(addNewEmptyAccountDto.CurrencyName);
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
        return Ok(accounts.OrderBy(x => x.CreatedDateTime));
    }

    [HttpGet]
    [Route("GetByNumber")]
    public async Task<ActionResult<AccountInfo>> GetAccountInfoByNumberAsync([FromBody] GetAccountInfoByNumberDto getAccountInfoByNumberDto)
        => await accountService.GetAccountInfoByNumber(getAccountInfoByNumberDto.AccountNumber);

    [HttpPatch]
    [Route("TransferMoney")]
    public async Task<IActionResult> TransferMoneyAsync([FromBody] TransferMoneyDto transferMoneyDto)
    {
        var currentUserAccounts = await accountService.GetAccounts(User.GetId());
        if (!currentUserAccounts.Select(x => x.AccountNumber).Contains(transferMoneyDto.AccountFromNumber))
        {
            return BadRequest("Можно переводить деньги только со своих счетов");
        }

        var transferResult = await accountService.TryTransferMoneyFromToAccount(transferMoneyDto.Amount,
                                                                                transferMoneyDto.AccountFromNumber,
                                                                                transferMoneyDto.AccountToNumber);
        if (transferResult == TransferMoneyResult.Failed)
        {
            return Problem("Произошла проблема с переводом. Перевод не выполнен");
        }

        if (transferResult == TransferMoneyResult.NotEnoughMoney)
        {
            return Problem("Недостаточно средств для перевода", statusCode: NotEnoughMoneyCode);
        }

        return Ok();
    }

    [HttpDelete]
    public async Task<IActionResult> CloseAsync([FromBody] CloseAccountDto closeAccountDto)
    {
        var wasRemoved = await accountService.TryRemoveAccountAsync(closeAccountDto.AccountNumber);
        if (!wasRemoved)
        {
            return BadRequest("Нельзя закрывать счета с остатками");
        }

        return Ok();
    }
}