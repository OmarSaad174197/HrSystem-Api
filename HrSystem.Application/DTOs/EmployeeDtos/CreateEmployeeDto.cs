namespace HrSystem.Application.DTOs.EmployeeDtos;

public class CreateEmployeeDto
{
    public string Name { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public int DepartmentId { get; set; }
}