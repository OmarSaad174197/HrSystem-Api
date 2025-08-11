# HrSystem-Api
This is an HR management system API built with .NET 9. It provides CRUD operations for Employees and Departments, manages vacation requests, and implements authentication and authorization using JWT . The project follows Clean Architecture principles, uses EF Core with Unit of Work and Repository patterns, and FluentValidation for input validation.

Prerequisites:
SQL Server or any compatible database
.Net 9

Configure database connection
Open appsettings.json and update the DefaultConnection string to match your database setup.

Apply database migrations
Create migration: dotnet ef migrations add InitialCreate --project HRSystem.Infrastructure --startup-project HRSystem.Api --output-dir Data/Migrations

Update database:dotnet ef database update --project HRSystem.Infrastructure --startup-project HRSystem.Api

Run the API: dotnet run --project HRSystem.Api

Using the API with Postman or other clients
Register a new user with /api/auth/register.
Login with /api/auth/login to receive a JWT token.
Use the token in the Authorization header as Bearer <token> to access protected endpoints.

Notes
Roles supported:
HR: Full access to manage employees, departments, and vacations.
Employee: Limited access; can request vacations for themselves only.
