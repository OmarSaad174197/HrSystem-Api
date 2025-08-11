using HrSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace HRSystem.Infrastructure.Configurations;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name).IsRequired().HasMaxLength(100);
        builder.Property(e => e.UserId).IsRequired().HasMaxLength(450); // Matches IdentityUser.Id
        builder.HasOne(e => e.Department).WithMany(d => d.Employees).HasForeignKey(e => e.DepartmentId).OnDelete(DeleteBehavior.Cascade);
    }
}