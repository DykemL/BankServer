using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BankServer.Models.DbEntities;
using BankServer.Models.DtoModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BankServer.Services.Auth;

public class AuthService : IAuthService
{
    private readonly UserManager<User> userManager;
    private readonly IJwtSecurityService jwtSecurityService;

    public AuthService(UserManager<User> userManager, IJwtSecurityService jwtSecurityService)
    {
        this.userManager = userManager;
        this.jwtSecurityService = jwtSecurityService;
    }

    public async Task<LoginResult?> Login([FromBody] LoginDto model)
    {
        var user = await userManager.FindByNameAsync(model.Username);
        if (user == null || await userManager.CheckPasswordAsync(user, model.Password))
        {
            return null;
        }

        var userRoles = await userManager.GetRolesAsync(user);

        var authClaims = new List<Claim>
        {
            new(ClaimTypes.Name, user.UserName),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        foreach (var userRole in userRoles)
        {
            authClaims.Add(new Claim(ClaimTypes.Role, userRole));
        }

        var token = jwtSecurityService.GetToken(authClaims);

        return new LoginResult
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            ExpirationDate = token.ValidTo
        };
    }

    public async Task<RegisterStatus> Register([FromBody] RegisterDto model)
    {
        var userExists = await userManager.FindByNameAsync(model.Username);
        if (userExists != null)
        {
            return RegisterStatus.AlreadyExists;
        }

        var user = new User()
        {
            UserName = model.Username,
            Email = model.Email,
            SecurityStamp = Guid.NewGuid().ToString()
        };
        var result = await userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
        {
            return RegisterStatus.Error;
        }

        return RegisterStatus.Success;
    }
}