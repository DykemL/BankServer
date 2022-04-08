using BankServer.Controllers.Types;
using BankServer.Models;
using BankServer.Models.DbEntities;
using Microsoft.EntityFrameworkCore;

namespace BankServer.Services;

public class CurrencyService
{
    private readonly AppDbContext appDbContext;

    public CurrencyService(AppDbContext appDbContext)
        => this.appDbContext = appDbContext;

    public async Task<Currency?> GetCurrency(string name)
        => await appDbContext.Currencies.SingleOrDefaultAsync(x => x.Name == name);

    public async Task<CurrencyInfo[]?> GetAllCurrencies()
        => await appDbContext.Currencies!.Select(x => new CurrencyInfo()
        {
            Id = x.Id,
            Name = x.Name,
            Power = x.Power
        }).ToArrayAsync();

    public long ConvertCurrency(long amount, Currency currencyFrom, Currency currencyTo)
        => amount * currencyFrom.Power / currencyTo.Power;
}