using HrSystem.Application.DTOs.EmployeeDtos;
namespace HRSystem.Application.Interfaces;

public interface IEmployeeService
{
    Task<IEnumerable<EmployeeDto>> GetAllAsync();
    Task<EmployeeDto?> GetByIdAsync(int id);
    Task<EmployeeDto> CreateAsync(CreateEmployeeDto dto);
    Task UpdateAsync(int id, UpdateEmployeeDto dto);
    Task DeleteAsync(int id);
    // To get employee's ID from user ID
    Task<int?> GetEmployeeIdByUserIdAsync(string userId); 
}