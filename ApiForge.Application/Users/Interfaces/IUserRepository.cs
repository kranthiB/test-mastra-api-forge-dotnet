
using ApiForge.Application.Common.Interfaces;
using ApiForge.Domain.Users;

namespace ApiForge.Application.Users.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<(IReadOnlyList<User> Items, long TotalCount)> GetPagedAsync(
        int offset,
        int limit,
        CancellationToken ct = default);

    Task<bool> ExistsByEmailAsync(string email, Guid? excludeId = null, CancellationToken ct = default);
}
