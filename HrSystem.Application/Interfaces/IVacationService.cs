using HrSystem.Application.DTOs.VacationDtos;
namespace HRSystem.Application.Interfaces;

public interface IVacationService
{
    // For single insert (employee or HR)
    Task<VacationDto> AddSingleAsync(CreateVacationDto dto, string currentUserId, string currentUserRole);

    // For batch insert (HR only)
    Task<IEnumerable<VacationDto>> AddBatchAsync(CreateVacationsBatchDto dto);

    // CRUD for vacations (HR only, for simplicity)
    Task<IEnumerable<VacationDto>> GetAllAsync();
    Task<VacationDto?> GetByIdAsync(int id);
    Task DeleteAsync(int id);
}