
using ApiForge.Application.Groups.DTOs;
using FluentValidation;
using System.Text.RegularExpressions;

namespace ApiForge.Application.Groups.Validators;

public class CreateGroupRequestValidator : AbstractValidator<CreateGroupRequest>
{
    public CreateGroupRequestValidator()
    {
        RuleFor(x => x.GroupName).NotEmpty();
        RuleFor(x => x.GroupSlug).NotEmpty().Must(BeKebabCase)
            .WithMessage("GroupSlug must be in kebab-case.");
        RuleFor(x => x.GroupDesc).NotEmpty();
    }

    private bool BeKebabCase(string slug)
    {
        if (string.IsNullOrEmpty(slug))
        {
            return false;
        }
        // Regex for kebab-case: starts with a letter, ends with a letter or number,
        // and contains only lowercase letters, numbers, and hyphens.
        return Regex.IsMatch(slug, @"^[a-z][a-z0-9-]*[a-z0-9]$");
    }
}
