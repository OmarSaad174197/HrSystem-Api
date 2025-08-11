using HrSystem.Application.DTOs.DepartmentDtos;
namespace HRSystem.Application.Interfaces;

public interface IDepartmentService
{
    Task<IEnumerable<DepartmentDto>> GetAllAsync();
    Task<DepartmentDto?> GetByIdAsync(int id);
    Task<DepartmentDto> CreateAsync(CreateDepartmentDto dto);
    Task UpdateAsync(int id, UpdateDepartmentDto dto);
    Task DeleteAsync(int id);
}