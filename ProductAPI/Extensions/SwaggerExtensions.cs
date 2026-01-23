using System.Reflection;
using Microsoft.OpenApi;
using ProductAPI.Infrastructure.Authentication;
using Swashbuckle.AspNetCore.Filters;

namespace ProductAPI.Extensions;

public static class SwaggerExtensions
{
    public static IServiceCollection AddSwaggerWithAuth0(this IServiceCollection services, IConfiguration configuration)
    {
        var auth0Settings = configuration
            .GetSection(Auth0Settings.SectionName)
            .Get<Auth0Settings>();

        if (auth0Settings == null)
        {
            throw new InvalidOperationException(
                "Auth0 settings are not configured in appsettings.json");
        }

        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Product API",
                Version = "v1",
                Description = "Product API with Auth0 Authentication"
            });

            // Use Authorization Code Flow (better for Swagger)
            options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    AuthorizationCode = new OpenApiOAuthFlow // ← Changed from Implicit
                    {
                        AuthorizationUrl =
                            new Uri(
                                $"https://{auth0Settings.Domain}/authorize?audience={auth0Settings.Audience}"),
                        TokenUrl = new Uri($"https://{auth0Settings.Domain}/oauth/token"),
                        Scopes = new Dictionary<string, string>
                        {
                            { "openid", "OpenID" },
                        }
                    }
                }
            });

            // Add the Security Requirement
            options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("oauth2", document)] = new List<string> { "openid" }
            });
            // Locate the XML file created by <GenerateDocumentationFile>
            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
            options.ExampleFilters();
            if (File.Exists(xmlPath))
            {
                options.IncludeXmlComments(xmlPath);
            }
        });

        return services;
    }
}