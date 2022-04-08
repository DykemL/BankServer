namespace BankServer.Models.DtoModels;

public class AccountInfo
{
    public string AccountNumber { get; set; }
    public Guid AccountId { get; set; }
    public Guid UserId { get; set; }
    public string CurrencyName { get; set; }
    public string CurrencyNameRus { get; set; }
    public decimal Money { get; set; }
    public DateTime CreatedDateTime { get; set; }
}