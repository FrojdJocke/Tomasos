using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TomasosASP.Models;

namespace TomasosASP.ViewModels
{
    public class TomasosModel
    {
        public Kund Customer { get; set; }
        public List<Matratt> Dishes { get; set; }
        public List<Produkt> Ingredients { get; set; }
        public List<MatrattProdukt> DishIngredientConnection { get; set; }
        public List<MatrattTyp> Types { get; set; }
        public int itemsInCart { get; set; }
        public List<BestallningMatratt> Cart { get; set; }
        public int TotalSum { get; set; }
    }
}
