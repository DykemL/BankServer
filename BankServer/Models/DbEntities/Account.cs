using System.ComponentModel.DataAnnotations;

namespace BankServer.Models.DbEntities;

public class Account : DbEntity
{
    [Required]
    public string AccountNumber { get; set; }
    [Required]
    public User User { get; set; }
    [Required]
    public decimal Money { get; set; }
    [Required]
    public Currency Currency { get; set; }
    [Required]
    public DateTime CreatedDateTime { get; set; }

    public Account()
        => CreatedDateTime = DateTime.UtcNow;
}