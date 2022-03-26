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

    public async Task AddNewEmptyAccountAsync(Guid userId)
    {
        var userIdString = userId.ToString();
        var user = await appDbContext.Users.FindAsync(userIdString).ConfigureAwait(false);
        if (user == null)
        {
            throw new NotFoundException();
        }
        user.Accounts.Add(new Account()
        {
            User = user
        });
        await appDbContext.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task<AccountDto[]> GetAccounts(Guid userId)
    {
        var userIdString = userId.ToString();
        var accounts = appDbContext.Accounts.Where(x => x.User.Id == userIdString);
        return await accounts.Select(x => new AccountDto()
        {
            UserId = userId,
            Money = x.Money,
            CreatedDateTime = x.CreatedDateTime
        }).ToArrayAsync().ConfigureAwait(false);
    }
}