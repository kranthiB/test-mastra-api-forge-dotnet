using ApiForge.Application.Users.DTOs;
using FluentValidation;

namespace ApiForge.Application.Users.Validators;

public sealed class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserRequestValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("UserName is required.")
            .MaximumLength(100).WithMessage("UserName must not exceed 100 characters.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email must be a valid email address.")
            .MaximumLength(256).WithMessage("Email must not exceed 256 characters.");

        RuleFor(x => x.DisplayName)
            .MaximumLength(200).WithMessage("DisplayName must not exceed 200 characters.")
            .When(x => x.DisplayName is not null);
    }
}
