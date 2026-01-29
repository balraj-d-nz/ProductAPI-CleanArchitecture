using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using ProductAPI.Application.Interfaces;
using ProductAPI.Domain.Entities;

namespace ProductAPI.Infrastructure.Persistence.Interceptors;

public class AuditableEntityInterceptor : SaveChangesInterceptor
{
    private readonly ICurrentUserService _currentUserService;

    // Inject the User Service here
    public AuditableEntityInterceptor(ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, 
        InterceptionResult<int> result, 
        CancellationToken cancellationToken = default)
    {
        var context = eventData.Context;
        if (context == null) return base.SavingChangesAsync(eventData, result, cancellationToken);
        
        // Get the current user ID (or "ProductAPI-System" if null)
        var userId = _currentUserService.UserId ?? "ProductApi-System";
        var entries = context.ChangeTracker.Entries().Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            if (entry.Entity is User user)
            {
                if (entry.State == EntityState.Added)
                {
                    user.CreatedAtUtc = DateTime.UtcNow;
                }
                else if (entry.State == EntityState.Modified)
                {
                    user.UpdatedAtUtc = DateTime.UtcNow;
                }
            }

            if (entry.Entity is Product product)
            {
                if (entry.State == EntityState.Added)
                {
                    product.CreatedAtUtc = DateTime.UtcNow;
                    product.CreatedById = userId;
                }
                else if (entry.State == EntityState.Modified)
                {
                    product.ModifiedAtUtc = DateTime.UtcNow;
                    product.ModifiedById = userId;
                }
            }
        }
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}