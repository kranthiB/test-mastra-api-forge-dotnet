
using ApiForge.Domain.Common;

namespace ApiForge.Domain.Users;

public sealed class User : AuditableEntity
{
    public string UserName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string? Cname { get; private set; }

    private User() { }

    public static User Create(string userName, string email, string? cname = null)
    {
        return new User
        {
            UserName = userName,
            Email = email,
            Cname = cname
        };
    }

    public void Update(string userName, string email)
    {
        UserName = userName;
        Email = email;
        Touch();
    }
}
