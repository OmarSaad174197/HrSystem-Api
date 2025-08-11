using HrSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace HRSystem.Infrastructure.Configurations;

public class VacationConfiguration : IEntityTypeConfiguration<Vacation>
{
    public void Configure(EntityTypeBuilder<Vacation> builder)
    {
        builder.HasKey(v => v.Id);
        builder.Property(v => v.Date).IsRequired();
        builder.Property(v => v.Reason).HasMaxLength(200);
        builder.HasOne(v => v.Employee)
            .WithMany(e => e.Vacations)
            .HasForeignKey(v => v.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}