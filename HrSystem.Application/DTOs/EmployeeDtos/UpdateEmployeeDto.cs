namespace HrSystem.Application.DTOs.EmployeeDtos;

public class UpdateEmployeeDto
{
    public string Name { get; set; } = string.Empty;
    public int DepartmentId { get; set; }
}