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
    public class ProductRepository : IProductRepository
    {
        private readonly IMapper _mapper;
        public ProductRepository(IMapper mapper)
        {
            _mapper = mapper;
        }

        public async Task<List<ProductDTO>> GetProducts()
        {
            return _mapper.Map<List<ProductDTO>>(await ProductDAO.GetProducts());
        }

        public async Task<List<ProductDTO>> GetProductsByCategory(int catId)
        {
            return _mapper.Map<List<ProductDTO>>(await ProductDAO.GetProductsByCategory(catId));
        }

        public async Task<ProductDTO> GetProductById(int id)
        {
            return _mapper.Map<ProductDTO>(await ProductDAO.GetProductById(id));
        }

        public async Task<ProductCreateUpdateDTO> CreateProduct(ProductCreateUpdateDTO productDTO)
        {
            var product = _mapper.Map<Product>(productDTO);
            return _mapper.Map<ProductCreateUpdateDTO>(await ProductDAO.CreateProduct(product));
        }

        public async Task<ProductCreateUpdateDTO> UpdateProduct(ProductCreateUpdateDTO productDTO)
        {
            var product = _mapper.Map<Product>(productDTO);
            return _mapper.Map<ProductCreateUpdateDTO>(await ProductDAO.UpdateProduct(product));
        }

        public async Task DeleteProduct(int id)
        {
            await ProductDAO.DeleteProduct(id);
        }
    }
}
