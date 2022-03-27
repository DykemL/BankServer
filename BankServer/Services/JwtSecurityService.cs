using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace BankServer.Services;

public class JwtSecurityService : IJwtSecurityService
{
    private readonly TimeSpan timeToLive = TimeSpan.FromHours(3);

    private readonly AppSettings appSettings;

    public JwtSecurityService(AppSettings appSettings)
        => this.appSettings = appSettings;

    public JwtSecurityToken CreateToken(List<Claim> authClaims)
    {
        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appSettings.JwtAuthSecretKey));

        var token = new JwtSecurityToken(
            appSettings.JwtAuthValidIssuer,
            appSettings.JwtAuthValidAudience,
            expires: DateTime.Now.Add(timeToLive),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );

        return token;
    }
}