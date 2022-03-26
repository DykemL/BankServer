namespace BankServer.Models.DtoModels;

public class AccountDto
{
    public Guid UserId { get; set; }
    public decimal Money { get; set; }
    public DateTime CreatedDateTime { get; set; }
}