using BankServer.Controllers.Types;
using BankServer.Helpers;
using BankServer.Services;
using Microsoft.AspNetCore.Mvc;

namespace BankServer.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces(HttpHeaders.JsonContentHeader)]
public class CurrencyController : ControllerBase
{
    private readonly CurrencyService currencyService;

    public CurrencyController(CurrencyService currencyService)
        => this.currencyService = currencyService;

    [HttpGet]
    [Route("All")]
    public async Task<ActionResult<CurrencyInfo[]>> GetAllCurrenciesAsync()
        => Ok(await currencyService.GetAllCurrencies());
}