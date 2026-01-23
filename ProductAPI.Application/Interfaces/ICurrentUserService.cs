namespace ProductAPI.Application.Interfaces;

public interface ICurrentUserService
{
    string? UserId { get; }
    string? Email { get; }
    bool IsAuthenticated { get; }
    IEnumerable<string> Permissions { get; }
}