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
    public class CustomerRepository : ICustomerRepository
    {
        private readonly IMapper _mapper;
        public CustomerRepository(IMapper mapper)
        {
            _mapper = mapper;
        }

        public async Task<List<CustomerDTO>> GetCustomers(string? searchString)
        {
            return _mapper.Map<List<CustomerDTO>>(await CustomerDAO.GetCustomers(searchString));
        }

        public async Task<CustomerDTO> GetCustomerById(string id)
        {
            return _mapper.Map<CustomerDTO>(await CustomerDAO.GetCustomerById(id));
        }

        public async Task<CustomerDTO> CreateCustomer(CustomerDTO customerDTO)
        {
            var customer = _mapper.Map<Customer>(customerDTO);
            return _mapper.Map<CustomerDTO>(await CustomerDAO.CreateCustomer(customer));
        }

        public async Task<CustomerDTO> UpdateCustomer(CustomerDTO customerDTO)
        {
            var customer = _mapper.Map<Customer>(customerDTO);
            return _mapper.Map<CustomerDTO>(await CustomerDAO.UpdateCustomer(customer));
        }

        public async Task DeleteCustomer(string id)
        {
            await CustomerDAO.DeleteCustomer(id);
        }

    }
}
