using BankServer.Models;
using BankServer.Services;
using BankServer.Services.Auth;

namespace BankServer;

public static class ContainerConfigurator
{
    public static void Configure(WebApplicationBuilder builder)
    {
        var services = builder.Services;
        services.AddScoped<IJwtSecurityService, JwtSecurityService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<AppDbInitializer>();
    }
}