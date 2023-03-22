using BusinessObject.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public interface IOrderRepository
    {
        Task<List<OrderDTO>> GetOrders();
        Task<OrderDTO> GetOrderById(int id);
        Task DeleteOrder(int id);
    }
}
