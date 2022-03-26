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
        => await appDbContext.Currencies.FirstOrDefaultAsync(x => x.Name == name);
}