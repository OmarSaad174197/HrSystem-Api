using FluentValidation;
using HrSystem.Application.DTOs;

namespace HRSystem.Application.Validators;

public class RegisterValidator : AbstractValidator<RegisterDto>
{
    public RegisterValidator()
    {
        RuleFor(x => x.Username).NotEmpty().MinimumLength(3);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
        RuleFor(x => x.Name).NotEmpty().When(x => x.Role == "Employee").WithMessage("Name required for Employee.");
        RuleFor(x => x.DepartmentId).GreaterThan(0).When(x => x.Role == "Employee").WithMessage("DepartmentId required for Employee.");
        RuleFor(x => x.Role).Must(r => r == "HR" || r == "Employee").WithMessage("Role must be HR or Employee.");
    }
}
