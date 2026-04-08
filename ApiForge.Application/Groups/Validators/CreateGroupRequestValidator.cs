using ApiForge.Application.Groups.DTOs;
using FluentValidation;

namespace ApiForge.Application.Groups.Validators;

public class CreateGroupRequestValidator : AbstractValidator<CreateGroupRequest>
{
    public CreateGroupRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Group name is required.")
            .MaximumLength(100)
            .WithMessage("Group name must not exceed 100 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(250)
            .WithMessage("Group description must not exceed 250 characters.");
    }
}
