using BankServer.Controllers.Types;
using BankServer.Models.DtoModels;
using BankServer.Models.Roles;
using BankServer.Services.Auth;
using Microsoft.AspNetCore.Mvc;

namespace BankServer.Controllers;

[ApiController]
[Route("api/[controller]")]
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
        var registerStatus = await authService.RegisterAsync(model, new [] { UserRoles.User });
        if (registerStatus == RegisterStatus.AlreadyExists)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new Response(ResponseStatus.Error, "Пользователь уже существует"));
        }

        if (registerStatus == RegisterStatus.Error)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new Response(ResponseStatus.Error, "Не удалось зарегистрировать пользователя"));
        }

        if (registerStatus == RegisterStatus.Success)
        {
            return Ok(new Response(ResponseStatus.Success, "Пользователь успешно зарегистрирован"));
        }

        return Ok(new Response(ResponseStatus.Error, "Неизвестная ошибка"));
    }
}