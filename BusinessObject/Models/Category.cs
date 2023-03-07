using System;
using System.Collections.Generic;

namespace BusinessObject.Models
{
    public partial class Category
    {
        public Category()
        {
            Products = new HashSet<Product>();
        }

        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public bool? IsActive { get; set; }
        public string CategoryGeneral { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
