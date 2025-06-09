using AutoMapper;
using Employee_Management_Backend.Models;
using Employee_Management_Backend.Models.DTOs;

namespace Employee_Management_Backend.Helper.Mapper
{
    public class EmployeeProfile : Profile
    {
        public EmployeeProfile()
        {
            CreateMap<Employee, EmployeeDTO>()
                .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department.DepartmentName));
        }
    }
}
