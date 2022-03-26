using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace BankServer.Services;

public class JwtSecurityService : IJwtSecurityService
{
    private readonly TimeSpan timeToLive = TimeSpan.FromHours(3);

    private readonly Settings settings;

    public JwtSecurityService(Settings settings)
        => this.settings = settings;

    public JwtSecurityToken GetToken(List<Claim> authClaims)
    {
        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.JwtAuthSecretKey));

        var token = new JwtSecurityToken(
            settings.JwtAuthValidIssuer,
            settings.JwtAuthValidAudience,
            expires: DateTime.Now.Add(timeToLive),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );

        return token;
    }
}