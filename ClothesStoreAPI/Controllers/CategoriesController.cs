using BusinessObject.DTO;
using Microsoft.AspNetCore.Mvc;
using Repository;

namespace ClothesStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : Controller
    {
        private ICategoryRepository repository;

        public CategoriesController(ICategoryRepository repo)
        {
            repository = repo;
        }

        //GET: api/Categories
        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            try
            {
                return StatusCode(200, await repository.GetCategories());
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

        //GET: api/Categories/id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            try
            {
                return StatusCode(200, await repository.GetCategoryById(id));
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
        public async Task<IActionResult> CreateCategory([FromBody] CategoryDTO categoryDTO)
        {
            try
            {
                return StatusCode(200, await repository.CreateCategory(categoryDTO));
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
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryDTO categoryDTO)
        {
            try
            {
                return StatusCode(200, await repository.UpdateCategory(categoryDTO));
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
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                await repository.DeleteCategory(id);
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
