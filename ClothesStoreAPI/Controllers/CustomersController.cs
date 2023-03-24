using BusinessObject.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repository;

namespace ClothesStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : Controller
    {
        private ICustomerRepository repository;

        public CustomersController(ICustomerRepository repo)
        {
            repository = repo;
        }

        //GET: api/Customers

        [HttpGet]
        public async Task<IActionResult> GetCustomers(string? searchString)
        {
            try
            {
                return StatusCode(200, await repository.GetCustomers(searchString));
            }
            catch (ApplicationException ae)
            {
                return StatusCode(400, ae.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        //GET: api/Customers/id
 
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomerById(string id)
        {
            try
            {
                return StatusCode(200, await repository.GetCustomerById(id));
            }
            catch (ApplicationException ae)
            {
                return StatusCode(400, ae.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        //POST

        [HttpPost]
        public async Task<IActionResult> CreateCustomer([FromBody] CustomerDTO customerDTO)
        {
            try
            {
                return StatusCode(200, await repository.CreateCustomer(customerDTO));
            }
            catch (ApplicationException ae)
            {
                return StatusCode(400, ae.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        //PUT

        [HttpPut]
        public async Task<IActionResult> UpdateCustomer([FromBody] CustomerDTO customerDTO)
        {
            try
            {
                return StatusCode(200, await repository.UpdateCustomer(customerDTO));
            }
            catch (ApplicationException ae)
            {
                return StatusCode(400, ae.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        //DELETE

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(string id)
        {
            try
            {
                await repository.DeleteCustomer(id);
                return StatusCode(204, "Delete successfully!");
            }
            catch (ApplicationException ae)
            {
                return StatusCode(400, ae.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
