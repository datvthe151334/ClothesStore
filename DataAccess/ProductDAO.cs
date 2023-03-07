using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class ProductDAO
    {
        public static async Task<List<Product>> GetProducts()
        {
            var listProducts = new List<Product>();
            try
            {
                using (var context = new ClothesStoreDBContext())
                {
                    listProducts = await context.Products.Include(x => x.Category).ToListAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return listProducts;
        }

        public static async Task<List<Product>> GetProductsByCategory(int catId)
        {
            var listProducts = new List<Product>();
            try
            {
                using (var context = new ClothesStoreDBContext())
                {
                    listProducts = await context.Products.Where(x => x.CategoryId == catId).Include(x => x.Category).ToListAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return listProducts;
        }

        public static async Task<Product> GetProductById(int id)
        {
            Product product = new Product();
            try
            {
                using (var context = new ClothesStoreDBContext())
                {
                    product = await context.Products.Include(x => x.Category).SingleOrDefaultAsync(x => x.ProductId == id);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return product;
        }

        public static async Task<Product> CreateProduct(Product product)
        {
            try
            {
                using (var context = new ClothesStoreDBContext())
                {
                    await context.Products.AddAsync(product);
                    await context.SaveChangesAsync();

                    return await context.Products.Include(x => x.Category).SingleOrDefaultAsync(x => x.ProductId == product.ProductId);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static async Task<Product> UpdateProduct(Product product)
        {
            try
            {
                using (var context = new ClothesStoreDBContext())
                {
                    context.Entry<Product>(product).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    await context.SaveChangesAsync();

                    return await context.Products.Include(x => x.Category).SingleOrDefaultAsync(x => x.ProductId == product.ProductId);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static async Task DeleteProduct(int id)
        {
            try
            {
                Product product = new Product();
                using (var context = new ClothesStoreDBContext())
                {
                    product = await context.Products.Include(x => x.Category).SingleOrDefaultAsync(x => x.ProductId == id);
                    product.IsActive = false;
                    context.Entry<Product>(product).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
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
