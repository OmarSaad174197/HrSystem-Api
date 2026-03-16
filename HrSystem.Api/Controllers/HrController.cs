using HrSystem.Application.DTOs.VacationDtos;
using HRSystem.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HRSystem.Api.Controllers;

[Route("api/admin/[controller]")]
[ApiController]
[Authorize(Roles = "HR")]
public class HrController : ControllerBase
{
    private readonly IVacationService _vacationService;

    public HrController(IVacationService vacationService)
    {
        _vacationService = vacationService;
    }

    [HttpPost("AddOneVacation")]
    public async Task<ActionResult<VacationDto>> AddSingleVacation(CreateVacationDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                     ?? throw new UnauthorizedAccessException("User ID not found.");
        var role = User.FindFirstValue(ClaimTypes.Role)
                   ?? throw new UnauthorizedAccessException("Role not found.");

        var vacation = await _vacationService.AddSingleAsync(dto, userId, role);
        return CreatedAtAction(nameof(GetVacationById), new { id = vacation.Id }, vacation);
    }

    [HttpPost("AddGroupOfVacations")]
    public async Task<ActionResult<List<VacationDto>>> AddBatchOfVacations(CreateVacationsBatchDto dto)
    {
        var created = await _vacationService.AddBatchAsync(dto);
        return Ok(created);
    }

    [HttpGet]
    public async Task<ActionResult<List<VacationDto>>> GetAllVacations()
    {
        var vacations = await _vacationService.GetAllAsync();
        return Ok(vacations);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<VacationDto>> GetVacationById(int id)
    {
        var vacation = await _vacationService.GetByIdAsync(id);
        return vacation == null ? NotFound() : Ok(vacation);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _vacationService.DeleteAsync(id);
        return NoContent();
    }
}
