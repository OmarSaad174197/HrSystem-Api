using HrSystem.Domain.Entities;
using HrSystem.Domain.Interfaces;
using HrSystem.Infrastructure.Data;

namespace HRSystem.Infrastructure.Repositories;

public class UnitOfWork(AppDbContext context) : IUnitOfWork
{
    private bool _disposed = false;

    public IGenericRepository<Department> Departments { get; } = new GenericRepository<Department>(context);
    public IGenericRepository<Employee> Employees { get; } = new GenericRepository<Employee>(context);
    public IGenericRepository<Vacation> Vacations { get; } = new GenericRepository<Vacation>(context);

    public async Task<int> SaveAsync()
    {
        return await context.SaveChangesAsync();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                context.Dispose();
            }
            _disposed = true;
        }
    }
}