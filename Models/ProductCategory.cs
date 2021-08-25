using System;
using System.Collections.Generic;

namespace TomasosASP.Models
{
    public partial class ProductCategory
    {
        public ProductCategory()
        {
            Products = new HashSet<Product>();
        }

        public int Id { get; set; }
        public string Description { get; set; }

        public ICollection<Product> Products { get; set; }
    }
}
