namespace ASP.NET_Identity.Models
{
    public enum Gender
    {
        Male,
        Female,
        Other
    }
    public class Employee
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = null!;
        public string Gender { get; set; } = null!;
        public List<int>SelectedDepartmentIDs { get; set; }
    }
    public class Department
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
    }
}
