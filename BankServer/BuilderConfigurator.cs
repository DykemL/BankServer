﻿using System.Text;
using BankServer.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace BankServer;

public static class BuilderConfigurator
{
    private const string AuthHeader = "Authorization";
    private const string AuthType = "Bearer";

    public static Settings GetConfigurationSettings(WebApplicationBuilder builder)
    {
        var configuration = builder.Configuration;
        var databaseConnectionString = Environment.GetEnvironmentVariable("DatabaseConnectionString")
                                       ?? configuration.GetConnectionString("RemoteConnection")
                                       ?? configuration.GetConnectionString("DefaultConnection");
        return new Settings()
        {
            DatabaseConnectionString = databaseConnectionString,
            JwtAuthValidAudience = configuration["JwtAuth:ValidAudience"],
            JwtAuthValidIssuer = configuration["JwtAuth:ValidIssuer"],
            JwtAuthSecretKey = configuration["JwtAuth:SecretKey"],
            AdminLogin = configuration["AuthData:AdminLogin"],
            AdminEmail = configuration["AuthData:AdminPassword"],
            AdminPassword = configuration["AuthData:AdminPassword"]
        };
    }

    public static void ConfigureJwt(WebApplicationBuilder builder, Settings settings)
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
                ValidAudience = settings.JwtAuthValidAudience,
                ValidIssuer = settings.JwtAuthValidIssuer,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.JwtAuthSecretKey))
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