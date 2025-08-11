// Validators/DepartmentValidators.cs
using FluentValidation;

using HrSystem.Application.DTOs.DepartmentDtos;

namespace HRSystem.Application.Validators;

public class CreateDepartmentValidator : AbstractValidator<CreateDepartmentDto>
{
    public CreateDepartmentValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.").MaximumLength(100).WithMessage("Name must not exceed 100 characters.");
    }
}