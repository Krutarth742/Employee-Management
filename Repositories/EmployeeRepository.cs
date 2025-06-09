using ClosedXML.Excel;
using Employee_Management_Backend.Data;
using Employee_Management_Backend.Helper;
using Employee_Management_Backend.Models;
using Employee_Management_Backend.Models.DTOs;
using Employee_Management_Backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using QRCoder;

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

        public async Task<MemoryStream> ExportEmployeesToExcel(List<EmployeeDTO> employees)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Employees");

            worksheet.Cell(1, 1).Value = "Name";
            worksheet.Cell(1, 2).Value = "Email";
            worksheet.Cell(1, 3).Value = "Phone";
            worksheet.Cell(1, 4).Value = "Age";
            worksheet.Cell(1, 5).Value = "Salary";
            worksheet.Cell(1, 6).Value = "DepartmentName";

            for (int i = 0; i < employees.Count; i++)
            {
                var employee = employees[i];
                worksheet.Cell(i + 2, 1).Value = employee.Name;
                worksheet.Cell(i + 2, 2).Value = employee.Email;
                worksheet.Cell(i + 2, 3).Value = employee.Phone;
                worksheet.Cell(i + 2, 4).Value = employee.Age;
                worksheet.Cell(i + 2, 5).Value = employee.Salary;
                worksheet.Cell(i + 2, 6).Value = employee.DepartmentName;
            }

            var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            return stream;
        }

        public async Task<List<Employee>> GetAllEmployees()
        {
            return await _context.Employees.Include(e => e.Department).ToListAsync();
        }

        public async Task<Employee> GetEmployeeByEmail(string email)
        {
            return await _context.Employees.FirstOrDefaultAsync(x => x.Email == email);
        }

        public async Task<Employee> GetEmployeeById(int id)
        {
            return await _context.Employees.FindAsync(id);
        }

        public async Task ImportEmployeesFromExcel(IFormFile file)
        {
            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            stream.Position = 0;

            using var workbook = new XLWorkbook(stream);
            var worksheet = workbook.Worksheet(1);
            var rows = worksheet.RowsUsed();

            var employees = new List<Employee>();

            foreach (var row in rows.Skip(1))
            {
                var employee = new Employee
                {
                    Name = row.Cell(1).GetValue<string>(),
                    Email = row.Cell(2).GetValue<string>(),
                    Password = PasswordHashHelper.HashPassword(row.Cell(3).GetValue<string>()),
                    Phone = row.Cell(4).GetValue<string>(),
                    Age = row.Cell(5).GetValue<int>(),
                    Salary = row.Cell(6).GetValue<int>()
                };
                employees.Add(employee);

                await _context.Employees.AddRangeAsync(employees);
            }
        }

        public async Task<Employee> LogInEmployee(LoginDTO loginDTO)
        {
            //return await _context.Employees.FirstOrDefaultAsync(x => x.Email == loginDTO.Email && x.Password == loginDTO.Password);
            return null;
        }

        public async Task SaveEmployee()
        {
            await _context.SaveChangesAsync();
        }

        public async Task UpdateEmployee(int id, Employee employee)
        {
            var emp = await _context.Employees.FindAsync(id);
            if (emp == null)
            {
                throw new Exception("Employee not found.");
            }
            emp.Name = employee.Name;
            emp.Phone = employee.Phone;
            emp.Age = employee.Age;
            emp.Salary = employee.Salary;
            emp.DepartmentID = employee.DepartmentID;
            _context.Employees.Update(emp);
        }

        public async Task<byte[]> GenerateEmployeeQRCode(Employee employee)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var employeeData = new
                    {
                        Id = employee.Id,
                        Name = employee.Name,
                        Email = employee.Email,
                        DepartmentID = employee.DepartmentID,
                        Phone = employee.Phone,
                    };
                    string employeeJson = System.Text.Json.JsonSerializer.Serialize(employeeData);

                    QRCodeGenerator qrGenerator = new QRCodeGenerator();
                    QRCodeData qrCodeData = qrGenerator.CreateQrCode(employeeJson, QRCodeGenerator.ECCLevel.Q);

                    // Instead of using the QRCode class (which might not implement IDisposable in newer versions)
                    // Use QRCode like this:
                    using (var qrCode = new PngByteQRCode(qrCodeData))
                    {
                        return qrCode.GetGraphic(20);
                    }

                    // Alternative approach if the above still doesn't work:
                    /*
                    using (var qrCode = new BitmapByteQRCode(qrCodeData))
                    {
                        return qrCode.GetGraphic(20);
                    }
                    */
                }
                catch (Exception ex)
                {
                    // Log the exception
                    throw new Exception($"Error generating QR code: {ex.Message}", ex);
                }
            });
        }
    }
}
