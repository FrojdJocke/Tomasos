using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TomasosASP.Models
{
    public partial class Product
    {
        public Product()
        {
            OrderItems = new HashSet<OrderItem>();
        }

        public int Id { get; set; }
        [Required(ErrorMessage = "Får ej vara tomt")]
        [MaxLength(50, ErrorMessage = "Maxlängd 50 tecken")]
        public string Name { get; set; }
        [MaxLength(200, ErrorMessage = "Maxlängd 200 tecken")]
        public string Description { get; set; }
        [Required]
        [Range(1,100000,ErrorMessage = "Felaktigt värde")]
        public int Price { get; set; }

        public ProductCategory Category { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
        public ICollection<Topping> Toppings { get; set; }
    }
}
