
using ApiForge.Application.GroupUserAssignments.DTOs;
using FluentValidation;

namespace ApiForge.Application.GroupUserAssignments.Validators;

public class CreateGroupUserAssignmentRequestValidator : AbstractValidator<CreateGroupUserAssignmentRequest>
{
    public CreateGroupUserAssignmentRequestValidator()
    {
        RuleFor(x => x.GroupId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
    }
}
