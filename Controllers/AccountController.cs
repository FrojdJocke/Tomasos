using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TomasosASP.Models;
using TomasosASP.ViewModels;
using TomasosASP.ViewModels.Auth;

namespace TomasosASP.Controllers
{
    [Authorize]
    public partial class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly TomasosContext _context;

        //Dependency Injection via konstruktorn
        public AccountController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            TomasosContext context
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }


        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(SignInViewModel model)
        {
            if (model.Username != null && model.Password!=null)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, false);

                if (result.Succeeded)
                {
                    //Om inloggningen gick bra visas startsidan
                    return RedirectToAction("Start", "Home");
                }
            }


            return View(model);
        }

        public async Task<IActionResult> LogOff()
        {
            await _signInManager.SignOutAsync();
            HttpContext.Session.Remove("Varukorg");
            return RedirectToAction("Start", "Home");
        }

        [AllowAnonymous]
        public IActionResult Register()
        {
            var model = new AccountModel();
            model.Unique = true;
            return View(model);
        }

        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Register(AccountModel user)
        {
            if (ModelState.IsValid)
            {
                if (_userManager.FindByNameAsync(user.Customer.UserName).Result != null)
                {
                    
                    user.Unique = false;
                    return View("Register", user);
                }
                //Lägger över värdena från sidan i en ApplicationUser klass
                var userIdentity = new User {UserName = user.Customer.UserName};

                //Skapar användaren i databasen
                var result = await _userManager.CreateAsync(userIdentity, user.Password);
                
                //Om det går bra loggas användaren in
                if (result.Succeeded)
                {
                    user.Customer.Name = user.Customer.Name.Trim();
                    user.Customer.Name = Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(user.Customer.Name.ToLower());

                    _context.Users.Add(user.Customer);

                    _context.SaveChanges();

                    //Sätter rollen på ny användare
                    await _userManager.AddToRoleAsync(userIdentity, "Regular");
                    await _signInManager.SignInAsync(userIdentity, isPersistent: false);

                    return RedirectToAction("Start", "Home");
                }
            }
            List<OrderItem> cart;
            if (HttpContext.Session.GetString("Varukorg") == null)
            {
                cart = null;
            }
            else
            {
                var serializedValue = HttpContext.Session.GetString("Varukorg");
                cart = JsonConvert.DeserializeObject<List<OrderItem>>(serializedValue);
            }

            int numberOfItems = 0;
            if (cart != null)
            {
                foreach (var item in cart)
                {
                    numberOfItems += item.Quantity;
                }
            }
            var model = new AccountModel()
            {
                Customer = user.Customer,
                itemsInCart = numberOfItems,
                Unique = true
            };


            return View(model);
        }


        public IActionResult AccountEdit()
        {
            var customer = _context.Users.SingleOrDefault(x => x.UserName == _userManager.GetUserName(User));

            List<OrderItem> cart;
            if (HttpContext.Session.GetString("Varukorg") == null)
            {
                cart = new List<OrderItem>();
            }
            else
            {
                //Hämta listan från Sessionen
                var serializedValue = HttpContext.Session.GetString("Varukorg");
                cart = JsonConvert.DeserializeObject<List<OrderItem>>(serializedValue);
            }

            int numberOfItems = 0;
            if (cart != null)
            {
                foreach (var item in cart)
                {
                    numberOfItems += item.Quantity;
                }
            }


            var temp = JsonConvert.SerializeObject(cart);
            HttpContext.Session.SetString("Varukorg", temp);

            var model = new AccountModel()
            {
                Customer = customer,
                itemsInCart = numberOfItems,
                Unique = true
            };

            return View(model);
        }

        //Updaterar och skickar till bekräftelse VY. Laddar om tidigare vy om misslyckat
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> EditConfirmation(AccountModel newInfo)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                
                newInfo.Customer.Name = Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(newInfo.Customer.Name.ToLower().Trim());

                //Sätt nya värden på kund & User
                user.Address = newInfo.Customer.Address;
                user.Name = newInfo.Customer.Name;
                user.Zip = newInfo.Customer.Zip;
                user.City = newInfo.Customer.City;
                user.Email = newInfo.Customer.Email;
                user.Telephone = newInfo.Customer.Telephone;
                user.UserName = newInfo.Customer.UserName;
                user.NormalizedUserName = user.UserName.ToUpper();
                //Spara
                _context.SaveChanges();

                //await _signInManager.SignOutAsync();
                //await _signInManager.PasswordSignInAsync(user.UserName, user.Password, true, false);

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


                var model = GetCustomer(prodList);

                return View(model);
            }
            newInfo.Unique = true;
            return View("AccountEdit",newInfo);
        }

        public TomasosModel GetCustomer(List<OrderItem> prodList)
        {
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
                Customer = _context.Users.SingleOrDefault(x => x.UserName == _userManager.GetUserName(User)),
                Products = _context.Products.ToList(),
                Ingredients = _context.Toppings.ToList(),
                ProductCatories = _context.ProductCategories.ToList(),
                itemsInCart = numberOfItems,
                Cart = prodList
            };

            return model;
        }
    }
}