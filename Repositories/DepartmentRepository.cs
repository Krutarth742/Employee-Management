using Employee_Management_Backend.Data;
using Employee_Management_Backend.Models;
using Employee_Management_Backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Employee_Management_Backend.Repositories
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly ApplicationDbContext _context;
        public DepartmentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddDepartment(Department department)
        {
            await _context.Departments.AddAsync(department);
        }

        public async Task<List<Department>> GetAllDepartments()
        {
            return await _context.Departments.ToListAsync();
        }
        public async Task SaveDepartment()
        {
            await _context.SaveChangesAsync();
        }
        public async Task UpdateDepartment(int DepartmentID, Department department)
        {
            var dep = await _context.Departments.FindAsync(DepartmentID);
            if (dep == null)
            {
                throw new Exception("Employee not found.");
            }
            else
            {
                dep.DepartmentName = department.DepartmentName;
                _context.Departments.Update(dep);
            }
        }

        public async Task<Department> GetDepartmentById(int DepartmentID)
        {
            return await _context.Departments.FindAsync(DepartmentID);
        }
        public async Task DeleteDepartment(int DepartmentID)
        {
            var dep = await _context.Departments.FindAsync(DepartmentID);
            _context.Departments.Remove(dep);
        }
    }
}
