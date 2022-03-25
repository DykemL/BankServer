namespace BankServer;

public class Settings
{ 
    public string DatabaseConnectionString { get; set; }
    public string JwtAuthValidAudience { get; set; }
    public string JwtAuthValidIssuer { get; set; }
    public string JwtAuthSecretKey { get; set; }
}