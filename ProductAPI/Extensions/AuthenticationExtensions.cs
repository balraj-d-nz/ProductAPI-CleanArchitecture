using Microsoft.AspNetCore.Authentication.JwtBearer;
using ProductAPI.Infrastructure.Authentication;

namespace ProductAPI.Extensions;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddAuth0Authentication(this IServiceCollection services,
        IConfiguration configuration)
    {
        // Bind and register Auth0 settings
        var auth0Settings = configuration
            .GetSection(Auth0Settings.SectionName)
            .Get<Auth0Settings>();

        if (auth0Settings == null)
        {
            throw new InvalidOperationException(
                "Auth0 settings are not configured properly in appsettings.json");
        }

        services.Configure<Auth0Settings>(
            configuration.GetSection(Auth0Settings.SectionName));

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = $"https://{auth0Settings.Domain}/";
                options.Audience = auth0Settings.Audience;
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true
                };
            });

        services.AddAuthorization(options =>
        {
            options.AddPolicy("ReadProduct", policy =>
                policy.RequireClaim("permissions", "read:product"));
        });
        return services;
    }
}