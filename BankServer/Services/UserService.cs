using BankServer.Controllers.Types;
using BankServer.Models.DbEntities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BankServer.Services;

public class UserService
{
    private readonly UserManager<User> userManager;

    public UserService(UserManager<User> userManager)
        => this.userManager = userManager;

    public async Task<UserInfo> GetUser(Guid userId)
    {
        var user = await userManager.Users.SingleOrDefaultAsync(x => x.Id == userId.ToString());
        return new UserInfo()
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email
        };
    }

    public async Task<UserInfo[]> GetAllUsers()
        => await userManager.Users.Select(x => new UserInfo
        {
            Id = x.Id,
            UserName = x.UserName,
            Email = x.Email
        }).ToArrayAsync();
}