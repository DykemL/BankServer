using BankServer.Models.DtoModels;
using Microsoft.AspNetCore.Mvc;

namespace BankServer.Services.Auth;

public interface IAuthService
{
    Task<LoginResult?> Login([FromBody] LoginDto model);
    Task<RegisterStatus> Register([FromBody] RegisterDto model);
}