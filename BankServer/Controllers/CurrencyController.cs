using BankServer.Controllers.Types;
using BankServer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BankServer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CurrencyController : ControllerBase
{
    private readonly AppDbContext appDbContext;

    public CurrencyController(AppDbContext appDbContext)
        => this.appDbContext = appDbContext;

    [HttpGet]
    [Route("GetAll")]
    public async Task<IActionResult> GetAllAsync()
        => Ok(await appDbContext.Currencies!.Select(x => new CurrencyInfo()
        {
            Id = x.Id,
            Name = x.Name,
            Power = x.Power
        }).ToArrayAsync());
}