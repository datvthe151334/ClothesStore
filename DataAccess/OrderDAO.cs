using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class OrderDAO
    {
        public static async Task<List<Order>> GetOrders(DateTime? startDate, DateTime? endDate)
        {
            var listOrders = new List<Order>();
            try
            {
                using (var context = new ClothesStoreDBContext())
                {
                    listOrders = await context.Orders.Include(x => x.Customer).Include(x => x.Employee).ToListAsync();

                    if(startDate != null)
                        listOrders = listOrders.Where(x => x.OrderDate >= startDate.Value).ToList();
                    if (endDate != null)
                        listOrders = listOrders.Where(x => x.OrderDate <= endDate.Value).ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return listOrders;
        }

        public static async Task<Order> GetOrderById(int id)
        {
            Order order = new Order();
            try
            {
                using (var context = new ClothesStoreDBContext())
                {
                    order = await context.Orders.Include(x => x.Customer).Include(x => x.Employee).SingleOrDefaultAsync(x => x.OrderId == id);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return order;
        }

        public static async Task<List<Order>> GetOrdersByCustomer(string customerId)
        {
            List<Order> Orders = new List<Order>();
            try
            {
                using (var context = new ClothesStoreDBContext())
                {
                    Orders = await context.Orders.Include(x => x.Customer).Include(x => x.Employee).Include(x => x.OrderDetails).ThenInclude(x => x.Product).Where(x => x.CustomerId == customerId).OrderByDescending(x => x.OrderDate).ToListAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return Orders;
        }

        public static async Task<Order> CreateOrder(Order order)
        {
            try
            {
                using (var context = new ClothesStoreDBContext())
                {
                    await context.Orders.AddAsync(order);
                    await context.SaveChangesAsync();

                    return await context.Orders.Include(x => x.Customer).Include(x => x.Employee).SingleOrDefaultAsync(x => x.OrderId == order.OrderId);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static async Task<Order> UpdateOrder(Order order)
        {
            try
            {
                using (var context = new ClothesStoreDBContext())
                {
                    context.Entry<Order>(order).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    await context.SaveChangesAsync();

                    return await context.Orders.Include(x => x.Customer).Include(x => x.Employee).SingleOrDefaultAsync(x => x.OrderId == order.OrderId);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static async Task DeleteOrder(int id)
        {
            try
            {
                Order? order = new Order();
                using (var context = new ClothesStoreDBContext())
                {
                    order = await context.Orders.SingleOrDefaultAsync(x => x.OrderId == id);
                    var listOrderDetails = await context.OrderDetails.Where(x => x.OrderId == id).ToListAsync();
                    context.OrderDetails.RemoveRange(listOrderDetails);
                    context.Orders.Remove(order);
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
