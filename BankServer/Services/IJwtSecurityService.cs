using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BankServer.Services;

public interface IJwtSecurityService
{
    JwtSecurityToken CreateToken(List<Claim> authClaims);
}