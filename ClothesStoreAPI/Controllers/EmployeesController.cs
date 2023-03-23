using BusinessObject.DTO;
using Microsoft.AspNetCore.Mvc;
using Repository;

namespace ClothesStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : Controller
    {
        private IEmployeeRepository repository;

        public EmployeesController(IEmployeeRepository repo)
        {
            repository = repo;
        }

        //GET: api/Employees
        [HttpGet]
        public async Task<IActionResult> GetEmployees(string? searchString)
        {
            try
            {
                return StatusCode(200, await repository.GetEmployees(searchString));
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

        //GET: api/Employees/id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployeeById(int id)
        {
            try
            {
                return StatusCode(200, await repository.GetEmployeeById(id));
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
        public async Task<IActionResult> CreateEmployee([FromBody] EmployeeCreateUpdateDTO employeeDTO)
        {
            try
            {
                return StatusCode(200, await repository.CreateEmployee(employeeDTO));
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
        public async Task<IActionResult> UpdateEmployee([FromBody] EmployeeCreateUpdateDTO employeeDTO)
        {
            try
            {
                return StatusCode(200, await repository.UpdateEmployee(employeeDTO));
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
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            try
            {
                await repository.DeleteEmployee(id);
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
