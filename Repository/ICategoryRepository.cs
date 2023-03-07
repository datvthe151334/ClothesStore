using BusinessObject.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public interface ICategoryRepository
    {
        Task<List<CategoryDTO>> GetCategories();
        Task<CategoryDTO> GetCategoryById(int id);
        Task<CategoryDTO> CreateCategory(CategoryDTO categoryDTO);
        Task<CategoryDTO> UpdateCategory(CategoryDTO categoryDTO);
        Task DeleteCategory(int id);
    }
}
