using FluentValidation;
using HrSystem.Application.DTOs.EmployeeDtos;
using HrSystem.Domain.Entities;
using HrSystem.Domain.Interfaces;
using HRSystem.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace HRSystem.Application.Services
{
    public class EmployeeService(IUnitOfWork unitOfWork, IValidator<CreateEmployeeDto> createValidator, IValidator<UpdateEmployeeDto> updateValidator, ILogger<EmployeeService> logger) : IEmployeeService
    {
        public async Task<IEnumerable<EmployeeDto>> GetAllAsync()
        {
            var employees = await unitOfWork.Employees.GetAllAsync();
            return employees.Select(e => new EmployeeDto { Id = e.Id, Name = e.Name, DepartmentId = e.DepartmentId, UserId = e.UserId });
        }

        public async Task<EmployeeDto?> GetByIdAsync(int id)
        {
            var employee = await unitOfWork.Employees.GetByIdAsync(id);
            return employee == null ? null : new EmployeeDto { Id = employee.Id, Name = employee.Name, DepartmentId = employee.DepartmentId, UserId = employee.UserId };
        }

        public async Task<EmployeeDto> CreateAsync(CreateEmployeeDto dto)
        {
            createValidator.ValidateAndThrow(dto);

            if (await unitOfWork.Departments.GetByIdAsync(dto.DepartmentId) == null)
                throw new KeyNotFoundException($"Department with ID {dto.DepartmentId} not found.");

            var employee = new Employee { Name = dto.Name, DepartmentId = dto.DepartmentId, UserId = dto.UserId };
            await unitOfWork.Employees.AddAsync(employee);
            await unitOfWork.SaveAsync();

            logger.LogInformation("Created Employee with ID {Id}", employee.Id);
            return new EmployeeDto { Id = employee.Id, Name = employee.Name, DepartmentId = employee.DepartmentId, UserId = employee.UserId };
        }

        public async Task UpdateAsync(int id, UpdateEmployeeDto dto)
        {
            updateValidator.ValidateAndThrow(dto);

            var employee = await unitOfWork.Employees.GetByIdAsync(id)
                           ?? throw new KeyNotFoundException($"Employee with ID {id} not found.");

            if (await unitOfWork.Departments.GetByIdAsync(dto.DepartmentId) == null)
                throw new KeyNotFoundException($"Department with ID {dto.DepartmentId} not found.");

            employee.Name = dto.Name;
            employee.DepartmentId = dto.DepartmentId;
            unitOfWork.Employees.Update(employee);
            await unitOfWork.SaveAsync();

            logger.LogInformation("Updated Employee with ID {Id}", id);
        }

        public async Task DeleteAsync(int id)
        {
            var employee = await unitOfWork.Employees.GetByIdAsync(id)
                           ?? throw new KeyNotFoundException($"Employee with ID {id} not found.");

            unitOfWork.Employees.Delete(employee);
            await unitOfWork.SaveAsync();

            logger.LogInformation("Deleted Employee with ID {Id}", id);
        }

        public async Task<int?> GetEmployeeIdByUserIdAsync(string userId)
        {
            var employees = await unitOfWork.Employees.GetAllAsync();
            var employee = employees.FirstOrDefault(e => e.UserId == userId);
            return employee?.Id;
        }
    }
}