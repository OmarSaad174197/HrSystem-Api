namespace HrSystem.Domain.Entities
{
    public class Vacation
    {
        public int Id { get; set; }
        public string Reason { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        // Nav Props
        public int EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }
    }
}