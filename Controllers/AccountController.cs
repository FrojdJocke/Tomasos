using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights.Extensibility.Implementation;
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
    public partial class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly TomasosContext _context;
        private readonly ApplicationDbContext _appContext;

        //Dependency Injection via konstruktorn
        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            TomasosContext context,
            ApplicationDbContext appContext
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _appContext = appContext;
        }


        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(Kund user)
        {
            var model = new Kund();

            if (user.AnvandarNamn != null && user.Losenord!=null)
            {
                var result = await _signInManager.PasswordSignInAsync(user.AnvandarNamn, user.Losenord, true, false);

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
                if (_context.Kund.SingleOrDefault(x => x.AnvandarNamn == user.Customer.AnvandarNamn) != null)
                {
                    user.Unique = false;
                    return View("Register", user);
                }
                //Lägger över värdena från sidan i en ApplicationUser klass
                var userIdentity = new ApplicationUser {UserName = user.Customer.AnvandarNamn};

                //Skapar användaren i databasen
                var result = await _userManager.CreateAsync(userIdentity, user.Customer.Losenord);
                
                //Om det går bra loggas användaren in
                if (result.Succeeded)
                {
                    user.Customer.Namn = user.Customer.Namn.Trim();
                    user.Customer.Namn = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(user.Customer.Namn.ToLower());

                    _context.Kund.Add(user.Customer);

                    _context.SaveChanges();

                    //Sätter rollen på ny användare
                    await _userManager.AddToRoleAsync(userIdentity, "Regular");
                    await _signInManager.SignInAsync(userIdentity, isPersistent: false);

                    return RedirectToAction("Start", "Home");
                }
            }
            List<BestallningMatratt> cart;
            if (HttpContext.Session.GetString("Varukorg") == null)
            {
                cart = null;
            }
            else
            {
                var serializedValue = HttpContext.Session.GetString("Varukorg");
                cart = JsonConvert.DeserializeObject<List<BestallningMatratt>>(serializedValue);
            }

            int numberOfItems = 0;
            if (cart != null)
            {
                foreach (var item in cart)
                {
                    numberOfItems += item.Antal;
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
            var customer = _context.Kund.SingleOrDefault(x => x.AnvandarNamn == _userManager.GetUserName(User));

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

            int numberOfItems = 0;
            if (cart != null)
            {
                foreach (var item in cart)
                {
                    numberOfItems += item.Antal;
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
                var customer = _context.Kund.SingleOrDefault(x => x.AnvandarNamn == _userManager.GetUserName(User));
                if (_context.Kund.SingleOrDefault(x => x.AnvandarNamn == newInfo.Customer.AnvandarNamn) != null)
                {
                    newInfo.Unique = false;
                    return View("AccountEdit", newInfo);
                }
                newInfo.Customer.Namn = newInfo.Customer.Namn.Trim();
                newInfo.Customer.Namn = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(newInfo.Customer.Namn.ToLower());

                //Sätt nya värden på kund & User
                customer.Gatuadress = newInfo.Customer.Gatuadress;
                customer.Namn = newInfo.Customer.Namn;
                customer.Postnr = newInfo.Customer.Postnr;
                customer.Postort = newInfo.Customer.Postort;
                customer.Email = newInfo.Customer.Email;
                customer.Telefon = newInfo.Customer.Telefon;
                customer.AnvandarNamn = newInfo.Customer.AnvandarNamn;
                var user = _userManager.Users.SingleOrDefault(x => x.UserName == _userManager.GetUserName(User));
                user.UserName = newInfo.Customer.AnvandarNamn;
                user.NormalizedUserName = user.UserName.ToUpper();
                //Spara
                _appContext.SaveChanges();
                _context.SaveChanges();

                await _signInManager.SignOutAsync();

                await _signInManager.PasswordSignInAsync(customer.AnvandarNamn, customer.Losenord, true, false);

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


                var model = GetCustomer(prodList);

                return View(model);
            }
            newInfo.Unique = true;
            return View("AccountEdit",newInfo);
        }

        public TomasosModel GetCustomer(List<BestallningMatratt> prodList)
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
                    numberOfItems += item.Antal;
                }
            }

            var model = new TomasosModel
            {
                Customer = _context.Kund.SingleOrDefault(x => x.AnvandarNamn == _userManager.GetUserName(User)),
                Dishes = _context.Matratt.ToList(),
                Ingredients = _context.Produkt.ToList(),
                DishIngredientConnection = _context.MatrattProdukt.ToList(),
                Types = _context.MatrattTyp.ToList(),
                itemsInCart = numberOfItems,
                Cart = prodList
            };

            return model;
        }
    }
}