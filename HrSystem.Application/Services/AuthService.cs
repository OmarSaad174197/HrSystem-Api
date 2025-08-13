using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FluentValidation;
using HrSystem.Application.DTOs;
using HrSystem.Application.DTOs.EmployeeDtos;
using HRSystem.Application.Interfaces;
using HrSystem.Domain.Interfaces;
using HrSystem.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace HRSystem.Application.Services
{
    public class AuthService(
        UserManager<AppUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IConfiguration config,
        IUnitOfWork unitOfWork,
        IEmployeeService employeeService,
        IValidator<RegisterDto> registerValidator,
        IValidator<LoginDto> loginValidator,
        ILogger<AuthService> logger
    ) : IAuthService
    {
        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
        {
            registerValidator.ValidateAndThrow(dto);

            // Check username existence
            if (await userManager.FindByNameAsync(dto.Username) != null)
                throw new InvalidOperationException("Username already exists.");

            // Validate role
            if (dto.Role != "HR" && dto.Role != "Employee")
                throw new ArgumentException("Invalid role.");

            // Create AppUser
            var user = new AppUser
            {
                UserName = dto.Username,
                Email = dto.Email
            };

            var result = await userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                throw new InvalidOperationException(string.Join(", ", result.Errors.Select(e => e.Description)));

            // Assign role
            if (!await roleManager.RoleExistsAsync(dto.Role))
                await roleManager.CreateAsync(new IdentityRole(dto.Role));

            await userManager.AddToRoleAsync(user, dto.Role);

            // Create Employee if role is Employee
            if (dto.Role == "Employee")
            {
                var createEmployeeDto = new CreateEmployeeDto
                {
                    Name = dto.Name,
                    DepartmentId = dto.DepartmentId,
                    UserId = user.Id
                };

                await employeeService.CreateAsync(createEmployeeDto);
            }

            logger.LogInformation("Registered user {Username} with role {Role}", dto.Username, dto.Role);

            return new AuthResponseDto
            {
                Token = GenerateJwtToken(user, dto.Role),
                Role = dto.Role
            };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            loginValidator.ValidateAndThrow(dto);

            var user = await userManager.FindByNameAsync(dto.Username)
                       ?? throw new UnauthorizedAccessException("Invalid credentials.");

            var isPasswordValid = await userManager.CheckPasswordAsync(user, dto.Password);
            if (!isPasswordValid)
                throw new UnauthorizedAccessException("Invalid credentials.");

            var roles = await userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault()
                       ?? throw new InvalidOperationException("User has no assigned role.");

            logger.LogInformation("User {Username} logged in successfully", dto.Username);

            return new AuthResponseDto
            {
                Token = GenerateJwtToken(user, role),
                Role = role
            };
        }

        private string GenerateJwtToken(AppUser user, string role)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName ?? ""),
                new Claim(ClaimTypes.Email, user.Email ?? ""),
                new Claim(ClaimTypes.Role, role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                config["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured.")
            ));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: config["Jwt:Issuer"],
                audience: config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
