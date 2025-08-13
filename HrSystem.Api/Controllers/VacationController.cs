using HRSystem.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using HrSystem.Application.DTOs.VacationDtos;

namespace HRSystem.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "HR,Employee")]
public class VacationController : ControllerBase
{
    private readonly IVacationService _vacationService;

    public VacationController(IVacationService vacationService)
    {
        _vacationService = vacationService;
    }

    [HttpPost]
    public async Task<ActionResult<VacationDto>> AddSingleVacation(CreateVacationDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                     ?? throw new UnauthorizedAccessException("User ID not found.");
        var role = User.FindFirstValue(ClaimTypes.Role)
                   ?? throw new UnauthorizedAccessException("Role not found.");

        var vacation = await _vacationService.AddSingleAsync(dto, userId, role);
        return CreatedAtAction(nameof(HrController.GetVacationById), "Hr", new { id = vacation.Id }, vacation);
    }
}