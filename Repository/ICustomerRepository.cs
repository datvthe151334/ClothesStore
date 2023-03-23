using BusinessObject.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public interface ICustomerRepository
    {
        Task<List<CustomerDTO>> GetCustomers(string? searchString);
        Task<CustomerDTO> GetCustomerById(string id);
        Task<CustomerDTO> CreateCustomer(CustomerDTO customerDTO);
        Task<CustomerDTO> UpdateCustomer(CustomerDTO customerDTO);
        Task DeleteCustomer(string id);
    }
}
