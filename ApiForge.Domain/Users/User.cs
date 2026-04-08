
using ApiForge.Domain.Common;

namespace ApiForge.Domain.Users;

public sealed class User : AuditableEntity
{
    public string UserName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string? Cname { get; private set; }

    private User() { } // Required for EF Core

    public static User Create(string userName, string email, string? cname)
    {
        // In a real app, you'd add validation here or in a domain service.
        return new User
        {
            UserName = userName,
            Email = email,
            Cname = cname,
        };
    }

    public void Update(string userName, string? cname)
    {
        // Email is not updatable in this example
        UserName = userName;
        Cname = cname;
        Touch();
    }
}
