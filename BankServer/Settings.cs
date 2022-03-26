namespace BankServer;

public class Settings
{ 
    public string DatabaseConnectionString { get; set; }

    public string JwtAuthValidAudience { get; set; }
    public string JwtAuthValidIssuer { get; set; }
    public string JwtAuthSecretKey { get; set; }

    public string AdminLogin { get; set; }
    public string AdminEmail { get; set; }
    public string AdminPassword { get; set; }
}