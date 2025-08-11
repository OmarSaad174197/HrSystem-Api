using FluentValidation;
using HrSystem.Application.DTOs.DepartmentDtos;

public class UpdateDepartmentValidator : AbstractValidator<UpdateDepartmentDto>
{
    public UpdateDepartmentValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.").MaximumLength(100).WithMessage("Name must not exceed 100 characters.");
    }
}