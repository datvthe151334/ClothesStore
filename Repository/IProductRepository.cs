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
        Task<List<ProductDTO>> GetProducts(int? categoryId, string? text, string? sortType, decimal? startPrice, decimal? endPrice, bool? isAdmin);
        Task<List<ProductDTO>> GetProductsByCategory(int catId);
        Task<List<ProductDTO>> GetProductsByCategoryGeneral(string catGeneral);
        Task<ProductDTO> GetProductById(int id);
        Task<ProductCreateUpdateDTO> CreateProduct(ProductCreateUpdateDTO productDTO);
        Task<bool> CreateProductMany(List<ProductCreateUpdateDTO> listProductDTO);
        Task<ProductCreateUpdateDTO> UpdateProduct(ProductCreateUpdateDTO productDTO);
        Task DeleteProduct(int id);
    }
}
