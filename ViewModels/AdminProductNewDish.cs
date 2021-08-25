using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using TomasosASP.Models;

namespace TomasosASP.ViewModels
{
    public class AdminProductNewDish
    {
        public List<Topping> Ingredients { get; set; }
        public List<SelectListItem> Types { get; set; }
        public List<Topping> SelectedIngredients { get; set; }

        public Product NewDish { get; set; }
        public int Type { get; set; }
    }
}
