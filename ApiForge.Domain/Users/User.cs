
using ApiForge.Domain.Common;

namespace ApiForge.Domain.Users;

public sealed class User : AuditableEntity
{
    public string UserName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string? DisplayName { get; private set; }
    public string Cname { get; private set; } = string.Empty;

    private User() { } // Required for EF Core

    public static User Create(string userName, string email, string cname, string? displayName)
    {
        return new User
        {
            UserName = userName,
            Email = email,
            Cname = cname,
            DisplayName = displayName,
        };
    }

    public void Update(string userName, string? displayName)
    {
        UserName = userName;
        DisplayName = displayName;
        Touch();
    }
}
