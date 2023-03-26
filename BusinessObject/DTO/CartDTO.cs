using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO
{
    public class CartDTO
    {
        public ProductDTO Product { get; set; }
        public int Quantity { get; set; }

        public CartDTO()
        {
        }

        public CartDTO(ProductDTO product, int quantity)
        {
            Product = product;
            Quantity = quantity;
        }
    }
}
