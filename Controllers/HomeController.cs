using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TomasosASP.Models;
using TomasosASP.ViewModels;

namespace TomasosASP.Controllers
{
    [Authorize]
    public partial class HomeController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly TomasosContext _context;

        //Dependency Injection via konstruktorn
        public HomeController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            TomasosContext context
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        [AllowAnonymous]
        public IActionResult Start()
        {
            if (!_signInManager.IsSignedIn(User))
            {
                var empty = new TomasosModel();
                return View(empty);
            }

            //if (userName != null)
            //{
            //    customer = _context.Kund.SingleOrDefault(x => x.AnvandarNamn == userName);
            //}
            //return View(customer);

            var dish = _context.Matratt.ToList();
            var ingredients = _context.Produkt.ToList();
            var con = _context.MatrattProdukt.ToList();
            var type = _context.MatrattTyp.ToList();
            var customer = _context.Kund.SingleOrDefault(x => x.AnvandarNamn == _userManager.GetUserName(User));
            List<BestallningMatratt> prodList;
            if (HttpContext.Session.GetString("Varukorg") == null)
            {
                prodList = null;
            }
            else
            {
                var serializedValue = HttpContext.Session.GetString("Varukorg");
                prodList = JsonConvert.DeserializeObject<List<BestallningMatratt>>(serializedValue);
            }

            int numberOfItems = 0;
            if (prodList == null)
            {
                numberOfItems = 0;
            }
            else
            {
                foreach (var item in prodList)
                {
                    numberOfItems += item.Antal;
                }
            }

            var model = new ViewModels.TomasosModel
            {

                Customer = customer,
                Dishes = dish,
                Ingredients = ingredients,
                DishIngredientConnection = con,
                Types = type,
                itemsInCart = numberOfItems,
                Cart = prodList
            };
            return View(model);
        }
        
        


    }
}
