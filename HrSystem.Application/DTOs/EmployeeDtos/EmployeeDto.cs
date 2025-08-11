namespace HrSystem.Application.DTOs.EmployeeDtos;

public class EmployeeDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int DepartmentId { get; set; }
    public string UserId { get; set; } = string.Empty;
}