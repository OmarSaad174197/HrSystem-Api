namespace HrSystem.Application.DTOs;

public class RegisterDto
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Role { get; set; } = "Employee"; // Default Role
    public string? Name { get; set; }
    public int DepartmentId { get; set; }
}
