
using ApiForge.Domain.Common;

namespace ApiForge.Domain.Users;

public sealed class User : AuditableEntity
{
    public string UserName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string? Cname { get; private set; }

    private User() { } // Private constructor for EF Core

    public static User Create(string userName, string email, string? cname = null)
    {
        // Basic validation can go here
        if (string.IsNullOrWhiteSpace(userName))
            throw new ArgumentException("Username cannot be empty.", nameof(userName));
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty.", nameof(email));

        return new User
        {
            UserName = userName,
            Email = email,
            Cname = cname
        };
    }

    public void Update(string userName, string email, string? cname = null)
    {
        if (string.IsNullOrWhiteSpace(userName))
            throw new ArgumentException("Username cannot be empty.", nameof(userName));
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty.", nameof(email));

        UserName = userName;
        Email = email;
        Cname = cname;
        Touch();
    }
}
