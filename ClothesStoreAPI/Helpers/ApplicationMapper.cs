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
                    opt => opt.MapFrom(src => src.Category.CategoryName))
                .ForMember(
                    dest => dest.CategoryGeneral,
                    opt => opt.MapFrom(src => src.Category.CategoryGeneral));
            CreateMap<ProductDTO, Product>();
            CreateMap<Product, ProductCreateUpdateDTO>().ReverseMap();

            //Category Mapper
            CreateMap<Category, CategoryDTO>().ReverseMap();

            //Customer Mapper
            CreateMap<Customer, CustomerDTO>().ReverseMap();

            //EmployeeMapper
            CreateMap<Employee, EmployeeDTO>()
                .ForMember(
                    dest => dest.DepartmentName,
                    opt => opt.MapFrom(src => src.Department.DepartmentName));
            CreateMap<EmployeeDTO, Employee>();
            CreateMap<Employee, EmployeeCreateUpdateDTO>().ReverseMap();

            //AccountMapper
            CreateMap<Account, AccountDTO>()
                .ForMember(
                    dest => dest.CustomerName,
                    opt => opt.MapFrom(src => src.Customer!.ContactName))
                .ForMember(
                    dest => dest.EmployeeName,
                    opt => opt.MapFrom(src => src.Employee!.LastName));
            CreateMap<AccountDTO, Account>();
        }
    }
}
