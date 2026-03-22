using ApiForge.Application.Common.Interfaces;
using ApiForge.Domain.Users;

namespace ApiForge.Application.Users.Interfaces;

/// <summary>
/// User-specific repository, extending the generic contract with
/// domain-specific query methods.
/// </summary>
public interface IUserRepository : IRepository<User>
{
    /// <summary>Returns a page of users ordered by creation date descending.</summary>
    Task<(IReadOnlyList<User> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        CancellationToken ct = default);

    /// <summary>
    /// Returns true if a user with the given user name already exists,
    /// optionally excluding a specific user (used for update checks).
    /// </summary>
    Task<bool> ExistsByUserNameAsync(
        string userName,
        Guid? excludeId = null,
        CancellationToken ct = default);

    /// <summary>
    /// Returns true if a user with the given email already exists,
    /// optionally excluding a specific user (used for update checks).
    /// </summary>
    Task<bool> ExistsByEmailAsync(
        string email,
        Guid? excludeId = null,
        CancellationToken ct = default);
}
