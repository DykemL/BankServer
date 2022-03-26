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
    private readonly RoleManager<IdentityRole> roleManager;
    private readonly IJwtSecurityService jwtSecurityService;

    public AuthService(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IJwtSecurityService jwtSecurityService)
    {
        this.userManager = userManager;
        this.roleManager = roleManager;
        this.jwtSecurityService = jwtSecurityService;
    }

    public async Task<LoginResult?> LoginAsync([FromBody] LoginDto model)
    {
        var user = await userManager.FindByNameAsync(model.Username).ConfigureAwait(false);
        if (user == null)
        {
            return null;
        }

        if (!await userManager.CheckPasswordAsync(user, model.Password).ConfigureAwait(false))
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

    public async Task<RegisterStatus> RegisterAsync([FromBody] RegisterDto model)
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