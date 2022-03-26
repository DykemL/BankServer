using System.ComponentModel.DataAnnotations;

namespace BankServer.Models.DbEntities;

public class Currency : DbEntity
{
    private const decimal DefaultPower = 1;

    [Required]
    public string Name { get; set; }
    [Required]
    public decimal Power { get; set; }

    public ICollection<Account> Accounts { get; set; }

    public Currency()
    {
        Power = DefaultPower;
        Accounts = new List<Account>();
    }
}