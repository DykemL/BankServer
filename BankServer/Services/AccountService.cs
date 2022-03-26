using BankServer.Controllers.Exceptions;
using BankServer.Models;
using BankServer.Models.DbEntities;
using BankServer.Models.DtoModels;
using Microsoft.EntityFrameworkCore;

namespace BankServer.Services;

public class AccountService
{
    private readonly AppDbContext appDbContext;

    public AccountService(AppDbContext appDbContext)
        => this.appDbContext = appDbContext;

    public async Task AddNewEmptyAccountAsync(Guid userId, Currency currency)
    {
        var userIdString = userId.ToString();
        var user = await appDbContext.Users.FindAsync(userIdString);
        if (user == null)
        {
            throw new NotFoundException();
        }
        user.Accounts.Add(new Account()
        {
            User = user,
            Currency = currency
        });
        await appDbContext.SaveChangesAsync();
    }

    public async Task<AccountInfo[]> GetAccounts(Guid userId)
    {
        var userIdString = userId.ToString();
        var accounts = appDbContext.Accounts.Where(x => x.User.Id == userIdString);
        return await accounts.Select(x => new AccountInfo()
        {
            AccountId = x.Id,
            UserId = userId,
            CurrencyName = x.Currency.Name,
            Money = x.Money,
            CreatedDateTime = x.CreatedDateTime
        }).ToArrayAsync();
    }

    public async Task TopUpAccount(Guid accountId, decimal amount)
    {
        var account = await appDbContext.Accounts.FirstOrDefaultAsync(x => x.Id == accountId);
        if (account == null)
        {
            throw new NotFoundException();
        }
        account.Money += amount;
        await appDbContext.SaveChangesAsync();
    }
}