using ApiForge.Application.Common.Interfaces;
using ApiForge.Domain.Users;

namespace ApiForge.Application.Users.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<bool> ExistsByEmailAsync(string email, Guid? excludeId = null, CancellationToken cancellationToken = default);
}
