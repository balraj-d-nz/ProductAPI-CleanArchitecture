using System.Security.Claims;
using ProductAPI.Domain.Entities;
using ProductAPI.Infrastructure.Persistence;

namespace ProductAPI.Middleware;

public class UserSyncMiddleware
{
    private readonly RequestDelegate _next;

    public UserSyncMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, DatabaseContext dbContext)
    {
        if (context.User?.Identity?.IsAuthenticated == true)
        {
            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userEmail = context.User.FindFirstValue(ClaimTypes.Email);
            var name = context.User.FindFirst("name")?.Value;
            
            if (!string.IsNullOrEmpty(userId))
            {
                var user = await dbContext.Users.FindAsync(userId);
                if (user == null)
                {
                    await dbContext.Users.AddAsync(new User
                    {
                        Id = userId,
                        Email = userEmail ?? "",
                        Name = name ?? "",
                        LastLoginAt = DateTime.UtcNow,
                        IsActive = true
                    });
                    await dbContext.SaveChangesAsync();
                }
                else
                {
                    var updateUser = false;
                    
                    if (user.Email != userEmail && !string.IsNullOrEmpty(userEmail))
                    {
                        user.Email = userEmail;
                        updateUser = true;
                    }
                    
                    if (user.Name != name && !string.IsNullOrEmpty(name))
                    {
                        user.Name = name;
                        updateUser = true;
                    }
                    
                    user.LastLoginAt = DateTime.UtcNow;
                    updateUser = true;
                    
                    if (updateUser)
                    {
                        user.ModifiedAtUtc = DateTime.UtcNow;
                        await dbContext.SaveChangesAsync();
                    }
                }
            }
        }

        await _next(context);

    }
}

// Extension method
public static class UserSyncMiddlewareExtensions
{
    public static IApplicationBuilder UseUserSync(this IApplicationBuilder app)
    {
        return app.UseMiddleware<UserSyncMiddleware>();
    }
}