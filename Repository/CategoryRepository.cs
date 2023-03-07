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
    public class CategoryRepository : ICategoryRepository
    {
        private readonly IMapper _mapper;
        public CategoryRepository(IMapper mapper)
        {
            _mapper = mapper;
        }

        public async Task<List<CategoryDTO>> GetCategories()
        {
            return _mapper.Map<List<CategoryDTO>>(await CategoryDAO.GetCategories());
        }

        public async Task<CategoryDTO> GetCategoryById(int id)
        {
            return _mapper.Map<CategoryDTO>(await CategoryDAO.GetCategoryById(id));
        }

        public async Task<CategoryDTO> CreateCategory(CategoryDTO categoryDTO)
        {
            var category = _mapper.Map<Category>(categoryDTO);
            return _mapper.Map<CategoryDTO>(await CategoryDAO.CreateCategory(category));
        }

        public async Task<CategoryDTO> UpdateCategory(CategoryDTO categoryDTO)
        {
            var category = _mapper.Map<Category>(categoryDTO);
            return _mapper.Map<CategoryDTO>(await CategoryDAO.UpdateCategory(category));
        }

        public async Task DeleteCategory(int id)
        {
            await CategoryDAO.DeleteCategory(id);
        }
    }
}
