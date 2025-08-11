namespace HrSystem.Application.DTOs.VacationDtos;

public class CreateVacationsBatchDto
{
    public List<CreateVacationDto> Vacations { get; set; } = new List<CreateVacationDto>();
}