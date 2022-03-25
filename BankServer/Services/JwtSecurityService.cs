using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace BankServer.Services;

public class JwtSecurityService
{
    private readonly IConfiguration configuration;

    public JwtSecurityService(IConfiguration configuration)
        => this.configuration = configuration;

    public JwtSecurityToken GetToken(List<Claim> authClaims)
    {
        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtAuth:Secret"]));

        var token = new JwtSecurityToken(
            configuration["Jwt:ValidIssuer"],
            configuration["Jwt:ValidAudience"],
            expires: DateTime.Now.AddHours(3),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );

        return token;
    }
}