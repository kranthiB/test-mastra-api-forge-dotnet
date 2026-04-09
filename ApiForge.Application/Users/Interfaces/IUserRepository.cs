using ApiForge.Application.Common.Interfaces;
using ApiForge.Domain.Users;

namespace ApiForge.Application.Users.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<bool> ExistsByEmailAsync(string email, Guid? excludeId = null, CancellationToken ct = default);
    Task<(IReadOnlyList<User> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, CancellationToken ct = default);
    Task<bool> HasGroupAssignmentsAsync(Guid userId, CancellationToken ct = default);
}
