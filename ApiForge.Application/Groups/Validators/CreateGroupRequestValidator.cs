
using ApiForge.Application.Groups.DTOs;
using FluentValidation;
using System.Text.RegularExpressions;

namespace ApiForge.Application.Groups.Validators;

public class CreateGroupRequestValidator : AbstractValidator<CreateGroupRequest>
{
    public CreateGroupRequestValidator()
    {
        RuleFor(x => x.GroupName)
            .NotEmpty().WithMessage("Group name is required.")
            .MaximumLength(100).WithMessage("Group name must not exceed 100 characters.");

        RuleFor(x => x.GroupSlug)
            .NotEmpty().WithMessage("Group slug is required.")
            .MaximumLength(120).WithMessage("Group slug must not exceed 120 characters.")
            .Must(BeKebabCase).WithMessage("Group slug must be in kebab-case.");

        RuleFor(x => x.GroupDesc)
            .NotEmpty().WithMessage("Group description is required.")
            .MaximumLength(500).WithMessage("Group description must not exceed 500 characters.");
    }

    private bool BeKebabCase(string slug)
    {
        if (string.IsNullOrEmpty(slug))
        {
            return false;
        }
        // Regex to check for lowercase letters, numbers, and hyphens,
        // and it doesn't start or end with a hyphen.
        return Regex.IsMatch(slug, @"^[a-z0-9]+(?:-[a-z0-9]+)*$");
    }
}
