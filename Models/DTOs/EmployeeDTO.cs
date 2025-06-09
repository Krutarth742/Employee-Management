namespace Employee_Management_Backend.Models.DTOs
{
    public class EmployeeDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public int Age { get; set; }
        public int Salary { get; set; }
        public int DepartmentID { get; set; }
        public string DepartmentName { get; set; }
    }
}
