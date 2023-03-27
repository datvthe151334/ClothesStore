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
    public class OrderRepository : IOrderRepository
    {
        private readonly IMapper _mapper;
        public OrderRepository(IMapper mapper)
        {
            _mapper = mapper;
        }

        public async Task<List<OrderDTO>> GetOrders(DateTime? startDate, DateTime? endDate)
        {
            return _mapper.Map<List<OrderDTO>>(await OrderDAO.GetOrders(startDate, endDate));
        }

        public async Task<OrderDictionaryDTO> GetOrderById(int id)
        {
            return _mapper.Map<OrderDictionaryDTO>(await OrderDAO.GetOrderById(id));
        }

        public async Task<List<OrderDictionaryDTO>> GetOrdersByCustomer(string customerId)
        {
            return _mapper.Map<List<OrderDictionaryDTO>> (await OrderDAO.GetOrdersByCustomer(customerId));
        }

        public async Task<OrderCreateUpdateDTO> CreateOrder(OrderCreateUpdateDTO orderDTO)
        {
            var order = _mapper.Map<Order>(orderDTO);
            return _mapper.Map<OrderCreateUpdateDTO>(await OrderDAO.CreateOrder(order));
        }

        public async Task DeleteOrder(int id)
        {
            await OrderDAO.DeleteOrder(id);
        }
    }
}
