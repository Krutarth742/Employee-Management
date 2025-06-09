using AutoMapper;
using Employee_Management_Backend.Helper;
using Employee_Management_Backend.Models;
using Employee_Management_Backend.Models.DTOs;
using Employee_Management_Backend.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Employee_Management_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public EmployeeController(IEmployeeRepository employeeRepository, IConfiguration configuration, IMapper mapper)
        {
            _employeeRepository = employeeRepository;
            _configuration = configuration;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllEmployees()
        {
            List<Employee> empList = await _employeeRepository.GetAllEmployees();
            var employeeDtos = _mapper.Map<List<EmployeeDTO>>(empList);
            return Ok(employeeDtos);
        }

        //[Authorize]
        [HttpGet("{Id}")]
        public async Task<IActionResult> GetEmployeeById([FromRoute] int Id)
        {
            return Ok(await _employeeRepository.GetEmployeeById(Id));
        }

        #region Add Employee
        [HttpPost]
        public async Task<IActionResult> AddEmployee([FromBody] Employee model)
        {
            model.Password = PasswordHashHelper.HashPassword(model.Password);
            await _employeeRepository.AddEmployee(model);
            await _employeeRepository.SaveEmployee();
            return Ok();
        }
        #endregion

        [HttpPut("{Id}")]
        public async Task<IActionResult> UpdateEmployee([FromRoute] int Id, [FromBody] Employee model)
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

        #region Log In
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            //var employee = await _employeeRepository.LogInEmployee(loginDTO);   
            var employee = await _employeeRepository.GetEmployeeByEmail(loginDTO.Email);
            if (employee == null)
            {
                return Unauthorized("Invalid email or password");
            }

            bool isPasswordCorrect = PasswordHashHelper.VerifyPassword(loginDTO.Password, employee.Password);
            if (!isPasswordCorrect)
            {
                return Unauthorized("Invalid email or password");
            }

            if (employee != null)
            {
                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim("Id", employee.Id.ToString()),
                    new Claim("Email", employee.Email.ToString())
                };
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(
                    _configuration["Jwt:Issuer"],
                    _configuration["Jwt:Audience"],
                    claims,
                    expires: DateTime.UtcNow.AddMinutes(60),
                    signingCredentials: signIn
                    );
                string tokenValue = new JwtSecurityTokenHandler().WriteToken(token);

                // Encrypt the token
                string encryptedToken = EncryptionHelper.EncryptString(tokenValue);

                return Ok(new { Token = encryptedToken, Employee = employee });
                //return Ok(employee);
            }
            return NoContent();
        }
        #endregion

        #region Export to Excel
        [HttpPost("export")]
        public async Task<IActionResult> ExportEmployees([FromBody] List<EmployeeDTO> employees)
        {
            var stream = await _employeeRepository.ExportEmployeesToExcel(employees);
            var content = stream.ToArray();
            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var fileName = "employees.xlsx";

            return File(content, contentType, fileName);
        }
        #endregion

        #region Import from Excel
        [HttpPost("import")]
        public async Task<IActionResult> ImportEmployees([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            await _employeeRepository.ImportEmployeesFromExcel(file);
            await _employeeRepository.SaveEmployee();
            return Ok("File imported successfully.");
        }
        #endregion

        [HttpGet("{Id}/qrcode")]
        public async Task<IActionResult> GenerateQRCode([FromRoute] int Id)
        {
            try
            {
                var employee = await _employeeRepository.GetEmployeeById(Id);
                if (employee == null)
                {
                    return NotFound($"Employee with ID {Id} not found");
                }

                byte[] qrCodeBytes = await _employeeRepository.GenerateEmployeeQRCode(employee);

                return File(qrCodeBytes, "image/png", $"employee-{Id}-qrcode.png");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
