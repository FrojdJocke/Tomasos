using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TomasosASP.Models;

namespace TomasosASP.ViewModels
{
    public class AdminProductEditDish
    {
        public Product Product { get; set; }
        public List<Topping> Toppings { get; set; }
        public List<Topping> NotUsedIngredients { get; set; }
    }
}
