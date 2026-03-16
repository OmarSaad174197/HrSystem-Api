using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using FluentValidation;
using HrSystem.Application.DTOs;
using HrSystem.Application.DTOs.DepartmentDtos;
using HrSystem.Application.DTOs.EmployeeDtos;
using HrSystem.Application.DTOs.VacationDtos;
using HRSystem.Application.Interfaces;
using HRSystem.Application.Services;
using HRSystem.Application.Validators;
using HrSystem.Api.Middleware;
using HrSystem.Domain.Interfaces;
using HrSystem.Domain.Entities;
using HrSystem.Infrastructure.Data;
using HRSystem.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Controllers & Endpoints
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity
builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
    {
        options.Password.RequiredLength = 6;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();


// JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrWhiteSpace(jwtKey))
    throw new InvalidOperationException("JWT Key is missing or empty.");

var issuerRaw = builder.Configuration["Jwt:Issuer"];
var audienceRaw = builder.Configuration["Jwt:Audience"];

var issuers = issuerRaw?
    .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
var audiences = audienceRaw?
    .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

if (issuers is not { Length: > 0 })
    throw new InvalidOperationException("Jwt:Issuer is missing or empty.");

if (audiences is not { Length: > 0 })
    throw new InvalidOperationException("Jwt:Audience is missing or empty.");

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.Zero,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };

        if (issuers.Length == 1)
            options.TokenValidationParameters.ValidIssuer = issuers[0];
        else
            options.TokenValidationParameters.ValidIssuers = issuers;

        if (audiences.Length == 1)
            options.TokenValidationParameters.ValidAudience = audiences[0];
        else
            options.TokenValidationParameters.ValidAudiences = audiences;
    });

builder.Services.AddAuthorization();

// Dependency Injection
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IVacationService, VacationService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Validators
builder.Services.AddScoped<IValidator<CreateDepartmentDto>, CreateDepartmentValidator>();
builder.Services.AddScoped<IValidator<UpdateDepartmentDto>, UpdateDepartmentValidator>();
builder.Services.AddScoped<IValidator<CreateEmployeeDto>, CreateEmployeeValidator>();
builder.Services.AddScoped<IValidator<UpdateEmployeeDto>, UpdateEmployeeValidator>();
builder.Services.AddScoped<IValidator<CreateVacationDto>, CreateVacationValidator>();
builder.Services.AddScoped<IValidator<RegisterDto>, RegisterValidator>();
builder.Services.AddScoped<IValidator<LoginDto>, LoginValidator>();

// Swagger config
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "HrVacationsSystem API", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

// Swagger Middleware
if (app.Environment.IsDevelopment())
{
    // as it is .net 9 so you must define these
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Seed Roles
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    string[] roles = { "HR", "Employee" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }
}

app.Run();
