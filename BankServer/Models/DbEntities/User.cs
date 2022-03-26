using Microsoft.AspNetCore.Identity;

namespace BankServer.Models.DbEntities;

public class User : IdentityUser
{
    public ICollection<Account> Accounts { get; set; }

    public User()
    {
        Accounts = new List<Account>();
        SecurityStamp = Guid.NewGuid().ToString();
    }
}