using Employee_Management_Backend.Models;

namespace Employee_Management_Backend.Repositories.Interfaces
{
    public interface IEmployeeRepository
    {
        Task<List<Employee>> GetAllEmployees();
        Task<Employee> GetEmployeeById(int id);
        Task AddEmployee(Employee employee);
        Task UpdateEmployee(int id, Employee employee);
        Task DeleteEmployee(int id);
        Task SaveEmployee();
    }
}
