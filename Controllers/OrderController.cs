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
    public class OrderController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly TomasosContext _context;

        //Dependency Injection via konstruktorn
        public OrderController
        (
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            TomasosContext context
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }


        public IActionResult Menu()
        {
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

        
        public IActionResult AddProduct2(int dishID, int customerID)
        {
            //Här läggs produkten till i varukorgen
            var product = _context.Matratt.SingleOrDefault(m => m.MatrattId == dishID);

            List<BestallningMatratt> cart;

            if (HttpContext.Session.GetString("Varukorg") == null)
            {
                cart = new List<BestallningMatratt>();
            }
            else
            {
                //Hämta listan från Sessionen
                var serializedValue = HttpContext.Session.GetString("Varukorg");
                cart = JsonConvert.DeserializeObject<List<BestallningMatratt>>(serializedValue);

            }
            if (cart.Any(x => x.MatrattId == dishID))
            {
                cart.SingleOrDefault(x => x.MatrattId == dishID).Antal++;
            }
            else
            {
                var addMore = new BestallningMatratt()
                {
                    Antal = 1,
                    MatrattId = dishID,
                    Matratt = product
                };

                cart.Add(addMore);
            }

            int numberOfItems = 0;
            foreach (var item in cart)
            {
                numberOfItems = numberOfItems + item.Antal;
            }
            
            //Lägga tillbaka listan i sessionen
            var temp = JsonConvert.SerializeObject(cart);
            HttpContext.Session.SetString("Varukorg", temp);

            var model = new ViewModels.TomasosModel
            {

                Customer = _context.Kund.SingleOrDefault(x => x.KundID == customerID),
                Dishes = _context.Matratt.ToList(),
                Ingredients = _context.Produkt.ToList(),
                DishIngredientConnection = _context.MatrattProdukt.ToList(),
                Types = _context.MatrattTyp.ToList(),
                Cart = cart,
                itemsInCart = numberOfItems
            };

            return View("Menu", model);
        }

        
        public IActionResult Checkout()
        {
            var serializedValue = HttpContext.Session.GetString("Varukorg");
            List<BestallningMatratt> cart;
            try
            {
                cart = JsonConvert.DeserializeObject<List<BestallningMatratt>>(serializedValue);
            }
            catch
            {
                cart = new List<BestallningMatratt>();
            }
            
            var kund = _userManager.GetUserName(User);
            var dish = _context.Matratt.ToList();

            int sum = cart.Sum(x => x.Matratt.Pris * x.Antal);

            var temp = JsonConvert.SerializeObject(cart);
            HttpContext.Session.SetString("Varukorg", temp);

            var model = new TomasosModel()
            {
                Customer = _context.Kund.SingleOrDefault(k => k.AnvandarNamn == kund),
                Cart = cart,
                Dishes = dish,
                TotalSum = sum
        };

            return View(model);
        }

        public IActionResult ConfirmOrder()
        {
            var serializedValue = HttpContext.Session.GetString("Varukorg");
            var cart = JsonConvert.DeserializeObject<List<BestallningMatratt>>(serializedValue);
            int sum = cart.Sum(x => x.Matratt.Pris * x.Antal);
            var customerName = _userManager.GetUserName(User);
            var customer = _context.Kund.SingleOrDefault(k => k.AnvandarNamn == customerName);
            
            var newOrder = new Bestallning()
            {
                BestallningDatum = DateTime.Now,
                Totalbelopp = sum,
                Levererad = false,
                KundId = customer.KundID
            };

            _context.Bestallning.Add(newOrder);
            _context.SaveChanges();

            foreach (var item in cart)
            {
                var newBeställningMaträtt = new BestallningMatratt()
                {
                    BestallningId = newOrder.BestallningId,
                    MatrattId = item.MatrattId,
                    Antal = item.Antal
                };
                _context.BestallningMatratt.Add(newBeställningMaträtt);
            }

            _context.SaveChanges();
            HttpContext.Session.Clear();

            return RedirectToAction("Menu");
        }
    }
}