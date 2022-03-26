using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BankServer.Services;

public interface IJwtSecurityService
{
    JwtSecurityToken GetToken(List<Claim> authClaims);
}