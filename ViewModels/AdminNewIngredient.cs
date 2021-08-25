using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TomasosASP.Models;

namespace TomasosASP.ViewModels
{
    public class AdminNewIngredient
    {
        public Topping NewIngredient { get; set; }
        public List<Topping> Ingredients { get; set; }
    }
}
