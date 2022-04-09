using System.Text;
using BankServer.Models;
using BankServer.Models.DbEntities;
using BankServer.Models.DtoModels;
using Microsoft.EntityFrameworkCore;

namespace BankServer.Services.Accounts;

public class AccountService
{
    private const int AccountNumberLength = 20;

    private readonly AppDbContext appDbContext;
    private readonly CurrencyService currencyService;

    public AccountService(AppDbContext appDbContext, CurrencyService currencyService)
    {
        this.appDbContext = appDbContext;
        this.currencyService = currencyService;
    }

    public async Task<bool> TryAddNewEmptyAccountAsync(Guid userId, Currency currency)
    {
        var user = await appDbContext.Users.FindAsync(userId.ToString());
        if (user == null)
        {
            return false;
        }
        user.Accounts.Add(new Account()
        {
            AccountNumber = await GenerateNewAccountNumber(),
            User = user,
            Currency = currency
        });
        await appDbContext.SaveChangesAsync();
        return true;
    }

    public async Task<AccountInfo> GetAccountInfoByNumber(string accountNumber)
    {
        var account = await appDbContext.Accounts!.Where(x => x.AccountNumber == accountNumber)
                                        .Include(x => x.Currency)
                                        .Include(x => x.User)
                                        .SingleOrDefaultAsync();
        if (account == null)
        {
            return null!;
        }

        return ToAccountInfo(account);
    }

    public async Task<AccountInfo[]> GetAccounts(Guid userId)
    {
        var userIdString = userId.ToString();
        var accounts = await appDbContext.Accounts!.Where(x => x.User.Id == userIdString)
                                         .Include(x => x.User)
                                         .Include(x => x.Currency)
                                         .ToArrayAsync();
        return accounts.Select(ToAccountInfo).ToArray();
    }

    public async Task<bool> TryRemoveAccountAsync(string accountNumber)
    {
        var account = await appDbContext.Accounts?.SingleOrDefaultAsync(x => x.AccountNumber == accountNumber)!;
        if (account?.Money > 0)
        {
            return false;
        }
        appDbContext.Accounts!.Remove(account!);
        await appDbContext.SaveChangesAsync();
        return true;
    }

    public async Task<TransferMoneyResult> TryTransferMoneyFromToAccount(long amount, string accountFromNumber, string accountToNumber)
    {
        var accountFrom = await appDbContext.Accounts!.Include(x => x.Currency)
                                      .SingleOrDefaultAsync(x => x.AccountNumber== accountFromNumber);
        var accountTo = await appDbContext.Accounts!.Include(x => x.Currency)
                                    .SingleOrDefaultAsync(x => x.AccountNumber == accountToNumber);

        if (accountFrom?.Money < amount)
        {
            return TransferMoneyResult.NotEnoughMoney;
        }
        var amountToConverted = currencyService.ConvertCurrency(amount, accountFrom!.Currency, accountTo!.Currency);
        if (amountToConverted <= 0)
        {
            return TransferMoneyResult.Failed;
        }
        var wasTransferSucceeded = await TryAddMoneyToAccountByNumberAsync(accountToNumber, amountToConverted);
        if (!wasTransferSucceeded)
        {
            return TransferMoneyResult.Failed;
        }

        await TryAddMoneyToAccountByNumberAsync(accountFromNumber, -amount);
        return TransferMoneyResult.Succeded;
    }

    public async Task<bool> TryAddMoneyToAccountAsync(Guid accountId, decimal amount)
    {
        var account = await appDbContext.Accounts!.FindAsync(accountId);
        if (account == null)
        {
            return false;
        }

        account.Money += amount;
        await appDbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> TryAddMoneyToAccountByNumberAsync(string accountNumber, decimal amount)
    {
        var account = await appDbContext.Accounts!.SingleOrDefaultAsync(x => x.AccountNumber == accountNumber);
        if (account == null)
        {
            return false;
        }

        account.Money += amount;
        await appDbContext.SaveChangesAsync();
        return true;
    }

    public async Task<string> GenerateNewAccountNumber()
    {
        var random = new Random();
        var accountNumberBuilder = new StringBuilder();
        for (var i = 0; i < AccountNumberLength; i++)
        {
            accountNumberBuilder.Append(random.NextInt64() % 10);
        }

        var accountNumber = accountNumberBuilder.ToString();
        if (await IsAccountNumberExists(accountNumber))
        {
            return await GenerateNewAccountNumber();
        }

        return accountNumberBuilder.ToString();
    }

    public async Task<bool> IsAccountNumberExists(string accountNumber)
        => await appDbContext.Accounts!.FirstOrDefaultAsync(x => x.AccountNumber == accountNumber) != null;

    private AccountInfo ToAccountInfo(Models.DbEntities.Account account)
        => new()
        {
            AccountNumber = account.AccountNumber,
            AccountId = account.Id,
            UserId = new Guid(account.User.Id),
            CurrencyName = account.Currency.Name,
            CurrencyNameRus = account.Currency.NameRus,
            Money = account.Money,
            CreatedDateTime = account.CreatedDateTime
        };
}