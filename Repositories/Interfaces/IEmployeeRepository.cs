using Employee_Management_Backend.Models;
using Employee_Management_Backend.Models.DTOs;

namespace Employee_Management_Backend.Repositories.Interfaces
{
    public interface IEmployeeRepository
    {
        Task<List<Employee>> GetAllEmployees();
        Task<Employee> GetEmployeeById(int id);
        Task AddEmployee(Employee employee);
        Task UpdateEmployee(int id, Employee employee);
        Task DeleteEmployee(int id);
        Task<Employee> LogInEmployee(LoginDTO loginDTO);
        Task<Employee> GetEmployeeByEmail(string email);
        Task<MemoryStream> ExportEmployeesToExcel(List<EmployeeDTO> employees);
        Task ImportEmployeesFromExcel(IFormFile file);
        Task<byte[]> GenerateEmployeeQRCode(Employee employee);
        Task SaveEmployee();
    }
}
