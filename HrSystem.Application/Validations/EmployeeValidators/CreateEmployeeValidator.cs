using FluentValidation;
using HrSystem.Application.DTOs.EmployeeDtos;
namespace HRSystem.Application.Validators;

public class CreateEmployeeValidator : AbstractValidator<CreateEmployeeDto>
{
    public CreateEmployeeValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.DepartmentId).GreaterThan(0).WithMessage("DepartmentId must be positive.");
        RuleFor(x => x.UserId).NotEmpty().WithMessage("UserId is required.");
    }
}