using ApiForge.Application.Groups.DTOs;
using FluentValidation;

namespace ApiForge.Application.Groups.Validators;

public class UpdateGroupRequestValidator : AbstractValidator<UpdateGroupRequest>
{
    public UpdateGroupRequestValidator()
    {
        RuleFor(x => x.GroupName)
            .NotEmpty().WithMessage("Group name is required.")
            .MaximumLength(100).WithMessage("Group name must not exceed 100 characters.");

        RuleFor(x => x.GroupDesc)
            .NotEmpty().WithMessage("Group description is required.")
            .MaximumLength(500).WithMessage("Group description must not exceed 500 characters.");
    }
}
