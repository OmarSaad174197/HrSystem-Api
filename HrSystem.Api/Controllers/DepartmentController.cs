using HrSystem.Application.DTOs.DepartmentDtos;
using HRSystem.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrSystem.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "HR")]
public class DepartmentController : ControllerBase
{
    private readonly IDepartmentService _departmentService;

    public DepartmentController(IDepartmentService departmentService)
    {
        _departmentService = departmentService;
    }

    [HttpGet]
    public async Task<ActionResult<List<DepartmentDto>>> GetAllDepartments()
    {
        var departments = await _departmentService.GetAllAsync();
        return Ok(departments);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<DepartmentDto>> GetDepartmentById(int id)
    {
        var department = await _departmentService.GetByIdAsync(id);
        return department == null ? NotFound() : Ok(department);
    }

    [HttpPost]
    public async Task<ActionResult<DepartmentDto>> CreateDepartment(CreateDepartmentDto dto)
    {
        var department = await _departmentService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetDepartmentById), new { id = department.Id }, department);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateDepartment(int id, UpdateDepartmentDto dto)
    {
        await _departmentService.UpdateAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteDepartment(int id)
    {
        await _departmentService.DeleteAsync(id);
        return NoContent();
    }
}