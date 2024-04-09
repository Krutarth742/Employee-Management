using Employee_Management_Backend.Models;
using Employee_Management_Backend.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Employee_Management_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeeController(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllEmployees()
        {
            List<Employee> empList = await _employeeRepository.GetAllEmployees();
            return Ok(empList);
        }

        [HttpGet("{Id}")]
        public async Task<IActionResult> GetEmployeeById([FromRoute] int Id)
        {
            return Ok(await _employeeRepository.GetEmployeeById(Id));
        }

        #region Add Employee
        [HttpPost]
        public async Task<IActionResult> AddEmployee([FromBody] Employee model)
        {
            await _employeeRepository.AddEmployee(model);
            await _employeeRepository.SaveEmployee();
            return Ok();
        }
        #endregion

        [HttpPut("{Id}")]
        public async Task<IActionResult> UpdateEmployee([FromRoute] int Id,[FromBody] Employee model)
        {
            await _employeeRepository.UpdateEmployee(Id, model);
            await _employeeRepository.SaveEmployee();
            return Ok();
        }

        [HttpDelete("{Id}")]
        public async Task<IActionResult> DeleteEmployee([FromRoute] int Id)
        {
            await _employeeRepository.DeleteEmployee(Id);
            await _employeeRepository.SaveEmployee();
            return Ok();
        }
    }
}
