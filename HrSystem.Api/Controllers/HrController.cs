using HrSystem.Application.DTOs.VacationDtos;
using HRSystem.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

    [HttpPost("Add one vacation")]
    public async Task<ActionResult<VacationDto>> AddSingleVacation(CreateVacationDto dto)
    {
        var vacation = await _vacationService.AddSingleAsync(dto, "admin", "HR");
        return CreatedAtAction(nameof(GetById), new { id = vacation.Id }, vacation);
    }

    [HttpPost("Add group of vacations")]
    public async Task<ActionResult<List<VacationDto>>> AddBatchOfVacations(CreateVacationsBatchDto dto)
    {
        var created = await _vacationService.AddBatchAsync(dto);
        return Ok(created);
    }

    [HttpGet]
    public async Task<ActionResult<List<VacationDto>>> GetAllVAcations()
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
    public async Task<IActionResult> DeleteVacation(int id)
    {
        await _vacationService.DeleteAsync(id);
        return NoContent();
    }
}
