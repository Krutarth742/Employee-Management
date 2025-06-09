using Employee_Management_Backend.Models;
using Employee_Management_Backend.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Employee_Management_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentRepository _departmentRepository;
        public DepartmentController(IDepartmentRepository departmentRepository)
        {
            _departmentRepository = departmentRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllDepartments()
        {
            List<Department> depList = await _departmentRepository.GetAllDepartments();
            return Ok(depList);
        }

        [HttpPost]
        public async Task<IActionResult> AddDepartment([FromBody] Department model)
        {
            await _departmentRepository.AddDepartment(model);
            await _departmentRepository.SaveDepartment();
            return Ok();
        }

        [HttpGet("{DepartmentID}")]
        public async Task<IActionResult> GetDepartmentById([FromRoute] int DepartmentID)
        {
            return Ok(await _departmentRepository.GetDepartmentById(DepartmentID));
        }

        [HttpPut("{DepartmentID}")]
        public async Task<IActionResult> UpdateDepartment([FromRoute] int DepartmentID, [FromBody] Department model)
        {
            await _departmentRepository.UpdateDepartment(DepartmentID, model);
            await _departmentRepository.SaveDepartment();
            return Ok();
        }

        [HttpDelete("{DepartmentID}")]
        public async Task<IActionResult> DeleteDepartment([FromRoute] int DepartmentID)
        {
            await _departmentRepository.DeleteDepartment(DepartmentID);
            await _departmentRepository.SaveDepartment();
            return Ok();
        }
    }
}
