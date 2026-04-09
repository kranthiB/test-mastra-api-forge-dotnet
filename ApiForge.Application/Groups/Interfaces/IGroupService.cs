
using ApiForge.Application.Common.Models;
using ApiForge.Application.Groups.DTOs;

namespace ApiForge.Application.Groups.Interfaces;

public interface IGroupService
{
    Task<Result<GroupResponse>> CreateAsync(CreateGroupRequest request, CancellationToken cancellationToken = default);
}
