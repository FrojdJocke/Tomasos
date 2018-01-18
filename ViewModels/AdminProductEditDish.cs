using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TomasosASP.Models;

namespace TomasosASP.ViewModels
{
    public class AdminProductEditDish
    {
        public Matratt Dish { get; set; }
        public List<MatrattProdukt> DishIngredients { get; set; }
        public List<Produkt> Ingredients { get; set; }
        public List<Produkt> NotUsedIngredients { get; set; }
    }
}
