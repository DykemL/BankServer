using System.Text;
using BankServer.Models;
using BankServer.Models.DbEntities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace BankServer;

public static class ApplicationConfigurator
{
    private const string AuthHeader = "Authorization";
    private const string AuthType = "Bearer";

    public static AppSettings GetConfigurationSettings(WebApplicationBuilder builder)
    {
        var configuration = builder.Configuration;
        var databaseConnectionString = Environment.GetEnvironmentVariable("DatabaseConnectionString")
                                       ?? configuration.GetConnectionString("RemoteConnection")
                                       ?? configuration.GetConnectionString("DefaultConnection");
        var jwtAuthSecretKey = Environment.GetEnvironmentVariable("JwtAuthSecretKey")
                               ?? configuration["JwtAuth:SecretKey"];
        return new AppSettings()
        {
            DatabaseConnectionString = databaseConnectionString,
            JwtAuthValidAudience = configuration["JwtAuth:ValidAudience"],
            JwtAuthValidIssuer = configuration["JwtAuth:ValidIssuer"],
            JwtAuthSecretKey = jwtAuthSecretKey,
            AdminLogin = configuration["AuthData:AdminLogin"],
            AdminEmail = configuration["AuthData:AdminEmail"],
            AdminPassword = configuration["AuthData:AdminPassword"]
        };
    }

    public static void ConfigureIdentity(WebApplicationBuilder builder)
    {
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
    }

    public static void ConfigureJwt(WebApplicationBuilder builder, AppSettings appSettings)
    {
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidAudience = appSettings.JwtAuthValidAudience,
                ValidIssuer = appSettings.JwtAuthValidIssuer,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appSettings.JwtAuthSecretKey!))
            };
        });
    }

    public static void ConfigureSwagger(WebApplicationBuilder builder)
    {
        builder.Services.AddSwaggerGen();
        builder.Services.ConfigureSwaggerGen(options =>
        {
            options.AddSecurityDefinition(AuthType, new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.ApiKey,
                In = ParameterLocation.Header,
                Name = AuthHeader
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new()
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = AuthType }
                    }, new string[] { }
                }
            });
            options.DescribeAllParametersInCamelCase();
        });
    }
}