using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BankServer.Controllers.Types;
using BankServer.Models.DbEntities;
using BankServer.Models.DtoModels;
using BankServer.Models.Roles;
using BankServer.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BankServer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<User> userManager;
    private readonly RoleManager<IdentityRole> roleManager;
    private readonly JwtSecurityService jwtSecurityService;

    public AuthController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, JwtSecurityService jwtSecurityService)
    {
        this.userManager = userManager;
        this.roleManager = roleManager;
        this.jwtSecurityService = jwtSecurityService;
    }

    [HttpPost]
    [Route("Login")]
    public async Task<IActionResult> Login([FromBody] LoginDto model)
    {
        var user = await userManager.FindByNameAsync(model.Username);
        if (user != null && await userManager.CheckPasswordAsync(user, model.Password))
        {
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

            authClaims.Add(new Claim(ClaimTypes.Role, "User"));
            var token = jwtSecurityService.GetToken(authClaims);

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo
            });
        }

        return Unauthorized();
    }

    [HttpPost]
    [Route("Register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto model)
    {
        var userExists = await userManager.FindByNameAsync(model.Username);
        if (userExists != null)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new Response(ResponseStatus.Error, "Пользователь уже существует"));
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
            return StatusCode(StatusCodes.Status500InternalServerError, new Response(ResponseStatus.Error, "Не удалось зарегистрировать пользователя"));
        }

        return Ok(new Response(ResponseStatus.Success, "Пользователь успешно зарегистрирован"));
    }
}