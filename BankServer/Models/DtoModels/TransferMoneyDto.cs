namespace BankServer.Models.DtoModels;

public class TransferMoneyDto
{
    public string AccountFromNumber { get; set; }
    public string AccountToNumber { get; set; }
    public long Amount { get; set; }
}