using AutoMapper;
using BusinessObject.DTO;
using BusinessObject.Models;

namespace ClothesStoreAPI.Helpers
{
    public class ApplicationMapper : Profile
    {
        public ApplicationMapper()
        {
            //Product Mapper
            CreateMap<Product, ProductDTO>()
                .ForMember(
                    dest => dest.CategoryName,
                    opt => opt.MapFrom(src => src.Category.CategoryName)
                );
            CreateMap<ProductDTO, Product>();
            CreateMap<Product, ProductCreateUpdateDTO>().ReverseMap();

            //Category Mapper
            CreateMap<Category, CategoryDTO>().ReverseMap();
        }
    }
}
