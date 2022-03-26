using BankServer.Models.DtoModels;
using Microsoft.AspNetCore.Mvc;

namespace BankServer.Services.Auth;

public interface IAuthService
{
    Task<LoginResult?> LoginAsync([FromBody] LoginDto model);
    Task<RegisterStatus> RegisterAsync([FromBody] RegisterDto model);
    Task<RegisterStatus> RegisterAsync([FromBody] RegisterDto model, string[] roles);
}