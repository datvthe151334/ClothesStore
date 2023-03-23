using BusinessObject.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public interface IEmployeeRepository
    {
        Task<List<EmployeeDTO>> GetEmployees(string? searchString);
        Task<EmployeeDTO> GetEmployeeById(int id);
        Task<EmployeeCreateUpdateDTO> CreateEmployee(EmployeeCreateUpdateDTO employeeDTO);
        Task<EmployeeCreateUpdateDTO> UpdateEmployee(EmployeeCreateUpdateDTO employeeDTO);
        Task DeleteEmployee(int id);
    }
}
