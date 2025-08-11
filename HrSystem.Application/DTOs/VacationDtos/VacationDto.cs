namespace HrSystem.Application.DTOs.VacationDtos;

public class VacationDto
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public DateTime Date { get; set; }
    public string Reason { get; set; } = string.Empty;
}