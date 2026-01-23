using System;
using System.Reflection;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi;
using ProductAPI.Application.Common.Mappings;
using ProductAPI.Application.DTOs;
using ProductAPI.Application.Interfaces;
using ProductAPI.Application.Services;
using ProductAPI.Domain.Entities;
using ProductAPI.Extensions;
using ProductAPI.Infrastructure.Authentication;
using ProductAPI.Infrastructure.Persistence;
using ProductAPI.Middleware;
using ProductAPI.Swagger.Filters;
using Swashbuckle.AspNetCore.Filters;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerExamplesFromAssemblies(Assembly.GetEntryAssembly());
builder.Services.AddAuth0Authentication(builder.Configuration); //Configures JWT Auth
builder.Services.AddSwaggerWithAuth0(builder.Configuration); //Configures Swagger

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

// Add DbContext
if (builder.Environment.IsEnvironment("Testing") == false)
{
    builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DbConnectionString")));
}

builder.Services.AddScoped<IApplicationDbContext>(provider =>
    provider.GetRequiredService<DatabaseContext>());

builder.Services.AddAutoMapper(config => {
    config.AddProfile<MappingProfile>();
});

var app = builder.Build();

app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

if (builder.Environment.IsEnvironment("Testing") == false)
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            var context = services.GetRequiredService<DatabaseContext>();
            await DatabaseSeeder.SeedDatabaseAsync(context);
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred during database seeding.");
        }
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(options =>
    {
        options.OpenApiVersion = OpenApiSpecVersion.OpenApi3_1;
    });
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Product API V1");
        c.OAuthClientId(builder.Configuration["Auth0:ClientId"]); // Add this to appsettings
        c.OAuthUsePkce();
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { } // Add this line at the bottom of the file to allow IntegrationTests Project to access Program.cs.