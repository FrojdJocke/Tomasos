using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TomasosASP.Models;
using TomasosASP.ViewModels;

namespace TomasosASP.Controllers
{
    public partial class LoggedInController : Controller
    {
        private TomasosContext _context;

        public LoggedInController(TomasosContext context)
        {
            _context = context;
        }

        public IActionResult StartLogin(Kund customer)
        {
            return View(customer);
        }

        public IActionResult Menu(int id)
        {
            var dish = _context.Matratt.ToList();
            var ingredients = _context.Produkt.ToList();
            var con = _context.MatrattProdukt.ToList();
            var type = _context.MatrattTyp.ToList();
            var customer = _context.Kund.SingleOrDefault(x => x.KundID == id);

            

            var model = new ViewModels.MenuModel
            {
                Name = customer.Namn,
                Customer = customer,
                Dishes = dish,
                Ingredients = ingredients,
                DishIngredientConnection = con,
                Types = type
            };
            return View(model);
        }


    }
}
