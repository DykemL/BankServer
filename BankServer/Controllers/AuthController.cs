using BankServer.Helpers;
using BankServer.Models.DtoModels;
using BankServer.Models.Roles;
using BankServer.Services.Auth;
using Microsoft.AspNetCore.Mvc;

namespace BankServer.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces(HttpHeaders.JsonContentHeader)]
public class AuthController : ControllerBase
{
    private readonly IAuthService authService;

    public AuthController(IAuthService authService)
        => this.authService = authService;

    [HttpPost]
    [Route("Login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginDto model)
    {
        var loginResult = await authService.LoginAsync(model);
        if (loginResult == null)
        {
            return Unauthorized();
        }

        return Ok(new
        {
            token = loginResult.Token,
            expiration = loginResult.ExpirationDate
        });
    }

    [HttpPost]
    [Route("Register")]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterDto model)
    {
        var registerStatus = await authService.RegisterAsync(model, UserRoles.User);
        if (registerStatus == RegisterStatus.AlreadyExists)
        {
            return BadRequest("Пользователь уже существует");
        }

        if (registerStatus == RegisterStatus.Error)
        {
            return BadRequest("Не удалось зарегистрировать пользователя");
        }

        if (registerStatus == RegisterStatus.Success)
        {
            return Ok("Пользователь успешно зарегистрирован");
        }

        return BadRequest("Неизвестная ошибка");
    }
}