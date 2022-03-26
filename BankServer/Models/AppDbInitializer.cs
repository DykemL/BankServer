using BankServer.Models.Roles;
using Microsoft.AspNetCore.Identity;

namespace BankServer.Models;

public class AppDbInitializer
{
    private readonly RoleManager<IdentityRole> roleManager;

    public AppDbInitializer(RoleManager<IdentityRole> roleManager)
        => this.roleManager = roleManager;

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
    }

    private async Task TryAddRoleAsync(string roleName)
    {
        if (await roleManager.FindByNameAsync(roleName) == null)
        {
            await roleManager.CreateAsync(new IdentityRole(roleName)).ConfigureAwait(false);
        }
    }
}