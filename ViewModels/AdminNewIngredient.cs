using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TomasosASP.Models;

namespace TomasosASP.ViewModels
{
    public class AdminNewIngredient
    {
        public Produkt NewIngredient { get; set; }
        public List<Produkt> Ingredients { get; set; }
    }
}
