using BankServer.Controllers.Types;
using BankServer.Models.DtoModels;
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
    public async Task<IActionResult> Login([FromBody] LoginDto model)
    {
        var loginResult = await authService.LoginAsync(model).ConfigureAwait(false);
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
    public async Task<IActionResult> Register([FromBody] RegisterDto model)
    {
        var registerStatus = await authService.RegisterAsync(model).ConfigureAwait(false);
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