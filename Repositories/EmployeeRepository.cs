using Employee_Management_Backend.Data;
using Employee_Management_Backend.Models;
using Employee_Management_Backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Employee_Management_Backend.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly ApplicationDbContext _context;
        public EmployeeRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task AddEmployee(Employee employee)
        {
            await _context.Employees.AddAsync(employee);
        }

        public async Task DeleteEmployee(int id)
        {
            var emp = await _context.Employees.FindAsync(id);
            _context.Employees.Remove(emp);
        }

        public async Task<List<Employee>> GetAllEmployees()
        {
            return await _context.Employees.ToListAsync();
        }

        public async Task<Employee> GetEmployeeById(int id)
        {
            return await _context.Employees.FindAsync(id);
        }

        public async Task SaveEmployee()
        {
            await _context.SaveChangesAsync();
        }

        public async Task UpdateEmployee(int id, Employee employee)
        {
            var emp = await _context.Employees.FindAsync(id);
            if(emp == null)
            {
                throw new Exception("Employee not found.");
            }
            emp.Name = employee.Name;
            emp.Phone = employee.Phone;
            emp.Age = employee.Age;
            emp.Salary = employee.Salary;
            _context.Employees.Update(emp);
        }
    }
}
