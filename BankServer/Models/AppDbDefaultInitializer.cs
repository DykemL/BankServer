using BankServer.Models.DbEntities;
using BankServer.Models.DtoModels;
using BankServer.Models.Roles;
using BankServer.Providers;
using BankServer.Services.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BankServer.Models;

public class AppDbInitializer
{
    private readonly IAuthService authService;
    private readonly RoleManager<IdentityRole> roleManager;
    private readonly AppDbContext appDbContext;
    private readonly Settings settings;

    public AppDbInitializer(IAuthService authService,
                            RoleManager<IdentityRole> roleManager,
                            AppDbContext appDbContext,
                            Settings settings)
    {
        this.authService = authService;
        this.roleManager = roleManager;
        this.appDbContext = appDbContext;
        this.settings = settings;
    }

    public static async Task InitializeAsync(WebApplication builder)
    {
        using var scopedServices = builder.Services.CreateScope();
        var serviceProvider = scopedServices.ServiceProvider;
        var appDbInitializer = serviceProvider.GetService<AppDbInitializer>();
        await appDbInitializer!.InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        await InitializeRoles();
        await InitializeUsers();
        await InitializeCurrency();
    }

    private async Task InitializeRoles()
    {
        await TryAddRoleAsync(UserRoles.User);
        await TryAddRoleAsync(UserRoles.Admin);
    }

    private async Task InitializeUsers()
        => await TryCreateAdmin();

    private async Task InitializeCurrency()
    {
        await TryAddCurrency(CurrencyHelper.GetDefaultRubbleCurrency());
        await TryAddCurrency(CurrencyHelper.GetDefaultDollarCurrency());
        await TryAddCurrency(CurrencyHelper.GetDefaultEuroCurrency());
    }

    private async Task TryAddRoleAsync(string roleName)
    {
        if (await roleManager.FindByNameAsync(roleName) == null)
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
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

    private async Task TryAddCurrency(Currency? newCurrency)
    {
        var currency = await appDbContext.Currencies.SingleOrDefaultAsync(x => x.Name == newCurrency.Name);
        if (currency != null)
        {
            return;
        }

        await appDbContext.Currencies.AddAsync(newCurrency);
        await appDbContext.SaveChangesAsync();
    }
}