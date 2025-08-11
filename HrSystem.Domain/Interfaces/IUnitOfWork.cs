using HrSystem.Domain.Entities;

namespace HrSystem.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IGenericRepository<Employee> Employees { get; }
    IGenericRepository<Department> Departments { get; }
    IGenericRepository<Vacation> Vacations { get; }
    
    Task<int> SaveAsync();
}