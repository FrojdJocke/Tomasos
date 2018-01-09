using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TomasosASP.Models;
using TomasosASP.ViewModels;

namespace TomasosASP.Controllers
{
    [Authorize]
    public partial class LoggedInController : Controller
    {
        private readonly TomasosContext _context;

        public LoggedInController(TomasosContext context)
        {
            _context = context;
        }

        public IActionResult StartLogin(Kund customer)
        {
            return View(customer);
        }
        public IActionResult StartLoginId(int id)
        {

            var customer = _context.Kund.SingleOrDefault(x => x.KundID == id);
            return RedirectToAction("StartLogin", customer);
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
                Id = id,
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
