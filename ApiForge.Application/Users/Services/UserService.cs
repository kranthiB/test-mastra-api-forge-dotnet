
using ApiForge.Application.Common.Models;
using ApiForge.Application.Users.DTOs;
using ApiForge.Application.Users.Interfaces;
using ApiForge.Domain.Users;
using FluentValidation;
using Microsoft.Extensions.Logging;
using System.Text;

namespace ApiForge.Application.Users.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IValidator<CreateUserRequest> _validator;
    private readonly ILogger<UserService> _logger;

    public UserService(IUserRepository userRepository, IValidator<CreateUserRequest> validator, ILogger<UserService> logger)
    {
        _userRepository = userRepository;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<UserResponse>> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = new StringBuilder();
            foreach (var error in validationResult.Errors)
            {
                errors.AppendLine(error.ErrorMessage);
            }
            return Result<UserResponse>.Validation(errors.ToString());
        }

        var emailExists = await _userRepository.ExistsByEmailAsync(request.Email, null, cancellationToken);
        if (emailExists)
        {
            return Result<UserResponse>.Conflict($"A user with email '{request.Email}' already exists.");
        }

        var user = User.Create(request.UserName, request.Email);

        await _userRepository.AddAsync(user, cancellationToken);

        _logger.LogInformation("User {UserId} created.", user.Id);

        var response = new UserResponse(user.Id, user.UserName, user.Email, user.Cname, user.CreatedAt, user.UpdatedAt);
        return Result<UserResponse>.Success(response);
    }

    public Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<PagedResult<UserResponse>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<UserResponse>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<UserResponse>> UpdateAsync(Guid id, CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
