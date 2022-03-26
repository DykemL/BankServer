using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BankServer.Models.DbEntities;
using BankServer.Models.DtoModels;
using BankServer.Models.Roles;
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

    public async Task<LoginResult?> LoginAsync(LoginDto model)
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
            new(JwtRegisteredClaimNames.NameId, user.Id),
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

    public async Task<RegisterStatus> RegisterAsync(RegisterDto model)
        => await RegisterAsync(model, new[] { UserRoles.User }).ConfigureAwait(false);

    public async Task<RegisterStatus> RegisterAsync(RegisterDto model, string[] roles)
    {
        var userExists = await userManager.FindByNameAsync(model.Username);
        if (userExists != null)
        {
            return RegisterStatus.AlreadyExists;
        }

        var user = new User()
        {
            UserName = model.Username,
            Email = model.Email
        };
        var result = await userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
        {
            return RegisterStatus.Error;
        }

        await userManager.AddToRolesAsync(user, roles).ConfigureAwait(false);

        return RegisterStatus.Success;
    }
}