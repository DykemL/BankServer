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

    public decimal ConvertCurrency(decimal amount, Currency currencyFrom, Currency currencyTo)
        => amount * currencyFrom.Power / currencyTo.Power;
}