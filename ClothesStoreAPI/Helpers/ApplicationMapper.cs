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
            CreateMap<AccountUpdateDTO, Account>().ReverseMap();

            //Order Mapper
            CreateMap<Order, OrderDTO>()
                .ForMember(
                    dest => dest.CustomerName,
                    opt => opt.MapFrom(src => src.Customer!.ContactName))
                .ForMember(
                    dest => dest.EmployeeName,
                    opt => opt.MapFrom(src => src.Employee!.LastName));
            CreateMap<OrderDTO, Order>();
            CreateMap<Order, OrderDictionaryDTO>()
                .ForPath(dest => dest.emp!.LastName,
                opt => opt.MapFrom(src => src.Employee!.LastName))
                 .ForPath(dest => dest.emp!.DepartmentName,
                opt => opt.MapFrom(src => src.Employee!.Department!.DepartmentName))
                .ForPath(dest => dest.emp!.Title,
                opt => opt.MapFrom(src => src.Employee!.Title))
                .ForPath(dest => dest.cus!.ContactName,
                opt => opt.MapFrom(src => src.Customer!.ContactName))
                .ForPath(dest => dest.cus!.CompanyName,
                opt => opt.MapFrom(src => src.Customer!.CompanyName))
                .ForPath(dest => dest.cus!.ContactTitle,
                opt => opt.MapFrom(src => src.Customer!.ContactTitle))
                .ForPath(dest => dest.cus!.Address,
                opt => opt.MapFrom(src => src.Customer!.Address))
                .ForMember(dest => dest.OrderDetails,
                opt => opt.MapFrom(src => src.OrderDetails));

            CreateMap<Order, OrderCreateUpdateDTO>()
                .ForMember(dest => dest.OrderDetails,
                opt => opt.MapFrom(src => src.OrderDetails));
            CreateMap<OrderCreateUpdateDTO, Order>();

            //OrderDetails Mapper
            CreateMap<OrderDetail, OrderDetailDTO>()
                .ForMember(
                    dest => dest.ProductName,
                    opt => opt.MapFrom(src => src.Product!.ProductName))
                .ForMember(
                    dest => dest.ProductPicture,
                    opt => opt.MapFrom(src => src.Product!.Picture));
            CreateMap<OrderDTO, Order>();
            CreateMap<OrderDetail, OrderDetailCreateUpdateDTO>().ReverseMap();
        }
    }
}
