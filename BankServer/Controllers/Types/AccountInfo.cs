namespace BankServer.Models.DtoModels;

public class AccountInfo
{
    public Guid AccountId { get; set; }
    public Guid UserId { get; set; }
    public string CurrencyName { get; set; }
    public decimal Money { get; set; }
    public DateTime CreatedDateTime { get; set; }
}