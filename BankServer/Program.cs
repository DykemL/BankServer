using BankServer;
using BankServer.Models;
using BankServer.Models.DbEntities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var settings = BuilderConfigurator.GetConfigurationSettings(builder);
builder.Services.AddScoped(_ => settings);

builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(settings.DatabaseConnectionString!));
builder.Services.AddIdentity<User, IdentityRole>(options =>
       {
           options.Password.RequireDigit = false;
           options.Password.RequireLowercase = false;
           options.Password.RequireUppercase = false;
           options.Password.RequiredLength = 0;
           options.Password.RequireNonAlphanumeric = false;
       })
       .AddEntityFrameworkStores<AppDbContext>()
       .AddDefaultTokenProviders();

builder.Services.AddControllers();
BuilderConfigurator.ConfigureJwt(builder, settings);

builder.Services.AddEndpointsApiExplorer();
BuilderConfigurator.ConfigureSwagger(builder);
ContainerConfigurator.Configure(builder);

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await AppDbInitializer.InitializeAsync(app);
app.Run();