using System.Collections.Concurrent;
using ApiForge.Application.Common.Interfaces;
using ApiForge.Domain.Common;

namespace ApiForge.Infrastructure.Persistence.Repositories;

/// <summary>
/// Thread-safe, generic in-memory repository backed by a <see cref="ConcurrentDictionary{TKey,TValue}"/>.
/// <para>
/// Swap this out for an EF Core or Cosmos DB implementation without touching
/// the Application or Domain layers — just register the new concrete type in
/// <see cref="ApiForge.Infrastructure.DependencyInjection"/>.
/// </para>
/// </summary>
public abstract class InMemoryRepository<T> : IRepository<T>
    where T : BaseEntity
{
    protected readonly ConcurrentDictionary<Guid, T> Store = new();

    public Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => Task.FromResult(Store.TryGetValue(id, out var entity) ? entity : null);

    public Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<T>>(Store.Values.ToList());

    public Task<T> AddAsync(T entity, CancellationToken ct = default)
    {
        Store[entity.Id] = entity;
        return Task.FromResult(entity);
    }

    public Task UpdateAsync(T entity, CancellationToken ct = default)
    {
        Store[entity.Id] = entity;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        Store.TryRemove(id, out _);
        return Task.CompletedTask;
    }

    public Task<bool> ExistsAsync(Guid id, CancellationToken ct = default)
        => Task.FromResult(Store.ContainsKey(id));
}
