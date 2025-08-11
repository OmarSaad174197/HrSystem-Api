namespace HrSystem.Domain.Entities
{
    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        // Nav props
        public int DepartmentId { get; set; }
        public Department Department { get; set; }
        public string UserId { get; set; } = string.Empty;
        public virtual List<Vacation> Vacations { get; set; } = new List<Vacation>();
    }
}