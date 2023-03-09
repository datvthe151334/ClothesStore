using BusinessObject.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public interface IProductRepository
    {
        Task<List<ProductDTO>> GetProducts();
        Task<List<ProductDTO>> GetProductsByCategory(int catId);
        Task<List<ProductDTO>> GetProductsByCategoryGeneral(string catGeneral);
        Task<ProductDTO> GetProductById(int id);
        Task<ProductCreateUpdateDTO> CreateProduct(ProductCreateUpdateDTO productDTO);
        Task<ProductCreateUpdateDTO> UpdateProduct(ProductCreateUpdateDTO productDTO);
        Task DeleteProduct(int id);
    }
}
