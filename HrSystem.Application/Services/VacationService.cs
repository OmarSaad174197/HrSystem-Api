using HrSystem.Application.DTOs.VacationDtos;
using HrSystem.Domain.Entities;
using HrSystem.Domain.Interfaces;
using HRSystem.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace HRSystem.Application.Services
{
    public class VacationService(IUnitOfWork unitOfWork, ILogger<VacationService> logger) : IVacationService
    {
        public async Task<VacationDto> AddSingleAsync(CreateVacationDto dto, string currentUserId, string currentUserRole)
        {
            if (currentUserRole != "HR" && currentUserRole != "Employee")
                throw new UnauthorizedAccessException("You do not have permission to add a vacation.");

            var vacation = new Vacation
            {
                EmployeeId = dto.EmployeeId,
                Date = dto.Date,
                Reason = dto.Reason
            };

            await unitOfWork.Vacations.AddAsync(vacation);
            await unitOfWork.SaveAsync();

            logger.LogInformation("Vacation created for Employee {EmployeeId} by {Role}", dto.EmployeeId, currentUserRole);

            return MapToDto(vacation);
        }

        public async Task<IEnumerable<VacationDto>> AddBatchAsync(CreateVacationsBatchDto dto)
        {
            if (dto.Vacations == null || !dto.Vacations.Any())
                throw new ArgumentException("No vacations provided.");

            var vacations = dto.Vacations.Select(v => new Vacation
            {
                EmployeeId = v.EmployeeId,
                Date = v.Date,
                Reason = v.Reason
            }).ToList();

            foreach (var vacation in vacations)
                await unitOfWork.Vacations.AddAsync(vacation);

            await unitOfWork.SaveAsync();

            logger.LogInformation("Batch vacation insert completed for {Count} records", vacations.Count);

            return vacations.Select(MapToDto);
        }

        public async Task<IEnumerable<VacationDto>> GetAllAsync()
        {
            var vacations = await unitOfWork.Vacations.GetAllAsync();
            return vacations.Select(MapToDto);
        }

        public async Task<VacationDto?> GetByIdAsync(int id)
        {
            var vacation = await unitOfWork.Vacations.GetByIdAsync(id);
            return vacation == null ? null : MapToDto(vacation);
        }

        public async Task DeleteAsync(int id)
        {
            var vacation = await unitOfWork.Vacations.GetByIdAsync(id)
                           ?? throw new KeyNotFoundException($"Vacation with ID {id} not found.");

            unitOfWork.Vacations.Delete(vacation);
            await unitOfWork.SaveAsync();

            logger.LogInformation("Vacation with ID {VacationId} deleted", id);
        }

        // Mapping helper method
        private static VacationDto MapToDto(Vacation vacation) =>
            new VacationDto
            {
                Id = vacation.Id,
                EmployeeId = vacation.EmployeeId,
                Date = vacation.Date,
                Reason = vacation.Reason
            };
    }
}
