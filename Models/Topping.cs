using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TomasosASP.Models
{
    public partial class Topping
    {
        public Topping()
        {
            Products = new HashSet<Product>();
        }

        public int Id { get; set; }
        [Required(ErrorMessage = "Produkten måste ha ett namn")]
        [MaxLength(50,ErrorMessage = "Maxlängd 50 tecken")]
        public string Name { get; set; }

        public ICollection<Product> Products { get; set; }
    }
}
