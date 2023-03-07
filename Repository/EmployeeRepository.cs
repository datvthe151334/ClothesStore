using AutoMapper;
using BusinessObject.DTO;
using BusinessObject.Models;
using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly IMapper _mapper;
        public EmployeeRepository(IMapper mapper)
        {
            _mapper = mapper;
        }

        public async Task<List<EmployeeDTO>> GetEmployees()
        {
            return _mapper.Map<List<EmployeeDTO>>(await EmployeeDAO.GetEmployees());
        }

        public async Task<EmployeeDTO> GetEmployeeById(int id)
        {
            return _mapper.Map<EmployeeDTO>(await EmployeeDAO.GetEmployeeById(id));
        }

        public async Task<EmployeeCreateUpdateDTO> CreateEmployee(EmployeeCreateUpdateDTO employeeDTO)
        {
            var employee = _mapper.Map<Employee>(employeeDTO);
            return _mapper.Map<EmployeeCreateUpdateDTO>(await EmployeeDAO.CreateEmployee(employee));
        }

        public async Task<EmployeeCreateUpdateDTO> UpdateEmployee(EmployeeCreateUpdateDTO employeeDTO)
        {
            var employee = _mapper.Map<Employee>(employeeDTO);
            return _mapper.Map<EmployeeCreateUpdateDTO>(await EmployeeDAO.UpdateEmployee(employee));
        }

        public async Task DeleteEmployee(int id)
        {
            await EmployeeDAO.DeleteEmployee(id);
        }
    }
}
