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
    public partial class HomeController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly TomasosContext _context;

        //Dependency Injection via konstruktorn
        public HomeController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            TomasosContext context
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        public IActionResult InitialStartup()
        {
            if (_signInManager.IsSignedIn(User))
            {
                _signInManager.SignOutAsync();
            }
            return RedirectToAction("Start");
        }

        public IActionResult Index()
        {
            if (!_signInManager.IsSignedIn(User))
            {
                var empty = new TomasosModel();
                return View(empty);
            }

            var dish = _context.Products.ToList();
            var ingredients = _context.Toppings.ToList();
            var type = _context.ProductCategories.ToList();
            var customer = _context.Users.SingleOrDefault(x => x.UserName == _userManager.GetUserName(User));
            List<OrderItem> prodList;
            if (HttpContext.Session.GetString("Varukorg") == null)
            {
                prodList = null;
            }
            else
            {
                var serializedValue = HttpContext.Session.GetString("Varukorg");
                prodList = JsonConvert.DeserializeObject<List<OrderItem>>(serializedValue);
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
                    numberOfItems += item.Quantity;
                }
            }

            var model = new TomasosModel
            {

                Customer = customer,
                Products = dish,
                Ingredients = ingredients,
                ProductCatories = type,
                itemsInCart = numberOfItems,
                Cart = prodList
            };
            return View(model);
        }

        public IActionResult ContactPage()
        {
            if (!_signInManager.IsSignedIn(User))
            {
                var empty = new TomasosModel();
                return View(empty);
            }

            var dish = _context.Products.ToList();
            var ingredients = _context.Toppings.ToList();
            var type = _context.ProductCategories.ToList();
            var customer = _context.Users.SingleOrDefault(x => x.UserName == _userManager.GetUserName(User));
            List<OrderItem> prodList;
            if (HttpContext.Session.GetString("Varukorg") == null)
            {
                prodList = null;
            }
            else
            {
                var serializedValue = HttpContext.Session.GetString("Varukorg");
                prodList = JsonConvert.DeserializeObject<List<OrderItem>>(serializedValue);
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
                    numberOfItems += item.Quantity;
                }
            }

            var model = new TomasosModel
            {

                Customer = customer,
                Products = dish,
                Ingredients = ingredients,
                ProductCatories = type,
                itemsInCart = numberOfItems,
                Cart = prodList
            };
            return View(model);

           
        }

        public IActionResult GetLoginModal()
        {
            var customer = new User();

            return PartialView("_LoginPartial", customer);
        }


    }
}
