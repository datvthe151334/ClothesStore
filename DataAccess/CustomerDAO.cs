using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class CustomerDAO
    {
        private static Random random = new Random();
        public static async Task<List<Customer>> GetCustomers(string? searchString)
        {
            var listCustomers = new List<Customer>();
            try
            {
                using (var context = new ClothesStoreDBContext())
                {
                    listCustomers = await context.Customers.ToListAsync();

                    if (searchString != null)
                    {
                        string txt = searchString.ToLower().Trim();
                        listCustomers = listCustomers.Where(x => x.ContactName.ToLower().Contains(txt)).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return listCustomers;
        }

        public static async Task<Customer> GetCustomerById(string id)
        {
            Customer customer = new Customer();
            try
            {
                using (var context = new ClothesStoreDBContext())
                {
                    customer = await context.Customers.SingleOrDefaultAsync(x => x.CustomerId == id);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return customer;
        }

        public static async Task<Customer> CreateCustomer(Customer customer)
        {
            try
            {
                using (var context = new ClothesStoreDBContext())
                {
                    customer.CustomerId = RandomString(5);
                    var cus = await context.Customers.SingleOrDefaultAsync(x => x.CustomerId == customer.CustomerId);
                    while(cus != null)
                    {
                        customer.CustomerId = RandomString(5);
                        cus = await context.Customers.SingleOrDefaultAsync(x => x.CustomerId == customer.CustomerId);
                    }
                    await context.Customers.AddAsync(customer);
                    await context.SaveChangesAsync();

                    return await context.Customers.SingleOrDefaultAsync(x => x.CustomerId == customer.CustomerId);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static async Task<Customer> UpdateCustomer(Customer customer)
        {
            try
            {
                using (var context = new ClothesStoreDBContext())
                {
                    context.Entry<Customer>(customer).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    await context.SaveChangesAsync();

                    return await context.Customers.SingleOrDefaultAsync(x => x.CustomerId == customer.CustomerId);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static async Task DeleteCustomer(string id)
        {
            try
            {
                Customer customer = new Customer();
                using (var context = new ClothesStoreDBContext())
                {
                    customer = await context.Customers.SingleOrDefaultAsync(x => x.CustomerId == id);
                    customer.IsActive = false;
                    context.Entry<Customer>(customer).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
