using System.ComponentModel.DataAnnotations;

namespace BankServer.Models.DbEntities;

public class Currency : DbEntity
{
    private const long DefaultPower = 1;

    [Required]
    public string Name { get; set; }
    [Required]
    public string NameRus { get; set; }
    [Required]
    public long Power { get; set; }

    public ICollection<Account> Accounts { get; set; }

    public Currency()
    {
        Power = DefaultPower;
        Accounts = new List<Account>();
    }
}