using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TomasosASP.Models;

namespace TomasosASP.ViewModels
{
    public class TomasosModel
    {
        public User Customer { get; set; }
        public List<Product> Products { get; set; }
        public List<Topping> Ingredients { get; set; }
        public List<ProductCategory> ProductCatories { get; set; }
        public int itemsInCart { get; set; }
        public List<OrderItem> Cart { get; set; }
        public int TotalSum { get; set; }
        public int Discount { get; set; }
    }
}
