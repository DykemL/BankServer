using BankServer.Models.DtoModels;
using BankServer.Models.Roles;
using BankServer.Services.Auth;
using Microsoft.AspNetCore.Identity;

namespace BankServer.Models;

public class AppDbInitializer
{
    private readonly IAuthService authService;
    private readonly RoleManager<IdentityRole> roleManager;
    private readonly Settings settings;

    public AppDbInitializer(IAuthService authService, RoleManager<IdentityRole> roleManager, Settings settings)
    {
        this.authService = authService;
        this.roleManager = roleManager;
        this.settings = settings;
    }

    public static async Task InitializeAsync(WebApplication builder)
    {
        using var scopedServices = builder.Services.CreateScope();
        var serviceProvider = scopedServices.ServiceProvider;
        var appDbInitializer = serviceProvider.GetService<AppDbInitializer>();
        await appDbInitializer!.InitializeAsync().ConfigureAwait(false);
    }

    private async Task InitializeAsync()
    {
        await TryAddRoleAsync(UserRoles.User).ConfigureAwait(false);
        await TryAddRoleAsync(UserRoles.Admin).ConfigureAwait(false);
        await TryCreateAdmin();
    }

    private async Task TryAddRoleAsync(string roleName)
    {
        if (await roleManager.FindByNameAsync(roleName) == null)
        {
            await roleManager.CreateAsync(new IdentityRole(roleName)).ConfigureAwait(false);
        }
    }

    private async Task TryCreateAdmin()
    {
        var registerDto = new RegisterDto()
        {
            Username = settings.AdminLogin,
            Email = settings.AdminEmail,
            Password = settings.AdminPassword
        };
        await authService.RegisterAsync(registerDto, new[] { UserRoles.User, UserRoles.Admin });
    }
}