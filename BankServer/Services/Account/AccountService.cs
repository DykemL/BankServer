using BankServer.Controllers.Exceptions;
using BankServer.Models;
using BankServer.Models.DbEntities;
using BankServer.Models.DtoModels;
using Microsoft.EntityFrameworkCore;

namespace BankServer.Services.Account;

public class AccountService
{
    private readonly AppDbContext appDbContext;
    private readonly CurrencyService currencyService;

    public AccountService(AppDbContext appDbContext, CurrencyService currencyService)
    {
        this.appDbContext = appDbContext;
        this.currencyService = currencyService;
    }

    public async Task<bool> TryAddNewEmptyAccountAsync(Guid userId, Currency currency)
    {
        var userIdString = userId.ToString();
        var user = await appDbContext.Users.FindAsync(userIdString);
        if (user == null)
        {
            return false;
        }
        user.Accounts.Add(new Models.DbEntities.Account()
        {
            User = user,
            Currency = currency
        });
        await appDbContext.SaveChangesAsync();
        return true;
    }

    public async Task<AccountInfo[]> GetAccounts(Guid userId)
    {
        var userIdString = userId.ToString();
        var accounts = appDbContext.Accounts!.Where(x => x.User.Id == userIdString);
        return await accounts.Select(x => new AccountInfo()
        {
            AccountId = x.Id,
            UserId = userId,
            CurrencyName = x.Currency.Name,
            Money = x.Money,
            CreatedDateTime = x.CreatedDateTime
        }).ToArrayAsync();
    }

    public async Task<bool> TryTransferMoneyFromToAccount(decimal amount, Guid accountFromId, Guid accountToId)
    {
        var accountFrom = await appDbContext.Accounts!.Include(x => x.Currency)
                                      .SingleOrDefaultAsync(x => x.Id == accountFromId);
        var accountTo = await appDbContext.Accounts!.Include(x => x.Currency)
                                    .SingleOrDefaultAsync(x => x.Id == accountToId);

        var amountToConverted = currencyService.ConvertCurrency(amount, accountFrom!.Currency, accountTo!.Currency);
        await AddMoneyToAccount(accountFromId, -amount);
        try
        {
            await AddMoneyToAccount(accountToId, amountToConverted);
        }
        catch (Exception)
        {
            await AddMoneyToAccount(accountFromId, amount);
            return false;
        }

        return true;
    }

    public async Task AddMoneyToAccount(Guid accountId, decimal amount)
    {
        var account = await appDbContext.Accounts!.FindAsync(accountId);
        if (account == null)
        {
            throw new NotFoundException();
        }

        account.Money += amount;
        await appDbContext.SaveChangesAsync();
    }
}