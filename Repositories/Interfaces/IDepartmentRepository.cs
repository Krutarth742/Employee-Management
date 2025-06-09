using Employee_Management_Backend.Models;

namespace Employee_Management_Backend.Repositories.Interfaces
{
    public interface IDepartmentRepository
    {
        Task<List<Department>> GetAllDepartments();
        Task AddDepartment(Department department);
        Task SaveDepartment();
        Task UpdateDepartment(int DepartmentID, Department department);
        Task<Department> GetDepartmentById(int DepartmentID);
        Task DeleteDepartment(int DepartmentID);
    }
}
