namespace BankServer.Services.Auth;

public class LoginResult
{
    public string? Token { get; set; }
    public DateTime ExpirationDate { get; set; }
}