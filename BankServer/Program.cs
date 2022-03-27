using BankServer;
using BankServer.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var settings = ApplicationConfigurator.GetConfigurationSettings(builder);
builder.Services.AddScoped(_ => settings);

builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(settings.DatabaseConnectionString!));
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

ApplicationConfigurator.ConfigureIdentity(builder);
ApplicationConfigurator.ConfigureJwt(builder, settings);
ApplicationConfigurator.ConfigureSwagger(builder);
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