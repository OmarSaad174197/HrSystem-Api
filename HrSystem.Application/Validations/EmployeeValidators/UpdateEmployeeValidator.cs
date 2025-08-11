using FluentValidation;
using HrSystem.Application.DTOs.EmployeeDtos;

public class UpdateEmployeeValidator : AbstractValidator<UpdateEmployeeDto>
{
    public UpdateEmployeeValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.DepartmentId).GreaterThan(0).WithMessage("DepartmentId must be positive.");
    }
}