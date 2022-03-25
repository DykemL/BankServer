using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace BankServer.Services;

public class JwtSecurityService
{
    private readonly Settings settings;

    public JwtSecurityService(Settings settings)
        => this.settings = settings;

    public JwtSecurityToken GetToken(List<Claim> authClaims)
    {
        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.JwtAuthSecretKey));

        var token = new JwtSecurityToken(
            settings.JwtAuthValidIssuer,
            settings.JwtAuthValidAudience,
            expires: DateTime.Now.AddHours(3),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );

        return token;
    }
}