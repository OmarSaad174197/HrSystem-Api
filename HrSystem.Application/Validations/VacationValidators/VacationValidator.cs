using FluentValidation;
using HrSystem.Application.DTOs.VacationDtos;

namespace HRSystem.Application.Validators;

public class CreateVacationValidator : AbstractValidator<CreateVacationDto>
{
    public CreateVacationValidator()
    {
        RuleFor(x => x.EmployeeId).GreaterThan(0).WithMessage("EmployeeId must be positive.");
        RuleFor(x => x.Date).GreaterThan(DateTime.Now).WithMessage("Vacation date must be in the future.");
        RuleFor(x => x.Reason).MaximumLength(200).WithMessage("Reason must not exceed 200 characters.");
    }
}