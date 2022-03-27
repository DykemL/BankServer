using BankServer.Controllers.Types;
using BankServer.Helpers;
using BankServer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BankServer.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces(HttpHeaders.JsonContentHeader)]
public class CurrencyController : ControllerBase
{
    private readonly AppDbContext appDbContext;

    public CurrencyController(AppDbContext appDbContext)
        => this.appDbContext = appDbContext;

    [HttpGet]
    [Route("All")]
    public async Task<ActionResult<CurrencyInfo[]>> GetAllCurrenciesAsync()
        => Ok(await appDbContext.Currencies!.Select(x => new CurrencyInfo()
        {
            Id = x.Id,
            Name = x.Name,
            Power = x.Power
        }).ToArrayAsync());
}