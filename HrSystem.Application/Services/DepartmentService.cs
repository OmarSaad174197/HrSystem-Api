using FluentValidation;
using HrSystem.Application.DTOs.DepartmentDtos;
using HrSystem.Domain.Entities;
using HrSystem.Domain.Interfaces;
using HRSystem.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace HRSystem.Application.Services
{
    public class DepartmentService(IUnitOfWork unitOfWork, IValidator<CreateDepartmentDto> createValidator, IValidator<UpdateDepartmentDto> updateValidator, ILogger<DepartmentService> logger) : IDepartmentService
    {
        public async Task<IEnumerable<DepartmentDto>> GetAllAsync()
        {
            var departments = await unitOfWork.Departments.GetAllAsync();
            return departments.Select(d => new DepartmentDto { Id = d.Id, Name = d.Name });
        }

        public async Task<DepartmentDto?> GetByIdAsync(int id)
        {
            var department = await unitOfWork.Departments.GetByIdAsync(id);
            return department == null ? null : new DepartmentDto { Id = department.Id, Name = department.Name };
        }

        public async Task<DepartmentDto> CreateAsync(CreateDepartmentDto dto)
        {
            createValidator.ValidateAndThrow(dto);

            var department = new Department { Name = dto.Name };
            await unitOfWork.Departments.AddAsync(department);
            await unitOfWork.SaveAsync();

            logger.LogInformation("Created Department with ID {Id}", department.Id);
            return new DepartmentDto { Id = department.Id, Name = department.Name };
        }

        public async Task UpdateAsync(int id, UpdateDepartmentDto dto)
        {
            updateValidator.ValidateAndThrow(dto);

            var department = await unitOfWork.Departments.GetByIdAsync(id)
                              ?? throw new KeyNotFoundException($"Department with ID {id} not found.");

            department.Name = dto.Name;
            unitOfWork.Departments.Update(department);
            await unitOfWork.SaveAsync();

            logger.LogInformation("Updated Department with ID {Id}", id);
        }

        public async Task DeleteAsync(int id)
        {
            var department = await unitOfWork.Departments.GetByIdAsync(id)
                              ?? throw new KeyNotFoundException($"Department with ID {id} not found.");

            unitOfWork.Departments.Delete(department);
            await unitOfWork.SaveAsync();

            logger.LogInformation("Deleted Department with ID {Id}", id);
        }
    }
}
