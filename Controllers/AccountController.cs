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

        //Dependency Injection via konstruktorn
        public AccountController(
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
        [HttpPost]
        public async Task<IActionResult> Login(TomasosModel user)
        {
            var model = new TomasosModel();

            var customer =
                _context.Kund.SingleOrDefault(x => x.AnvandarNamn == user.Customer.AnvandarNamn && x.Losenord == user.Customer.Losenord);


            var result = await _signInManager.PasswordSignInAsync(user.Customer.AnvandarNamn, user.Customer.Losenord, true, false);

            if (result.Succeeded)
            {
                //Om inloggningen gick bra visas startsidan
                return RedirectToAction("Start", "Home");
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
            var model = new Kund();
            return View(model);
        }

        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Register(TomasosModel user)
        {
            if (ModelState.IsValid)
            {
                if (_context.Kund.SingleOrDefault(x => x.AnvandarNamn == user.Customer.AnvandarNamn) != null)
                {
                    ModelState.AddModelError("AnvandarNamn", "Användarnamnet finns redan");
                    return View();
                }
                //Lägger över värdena från sidan i en ApplicationUser klass
                var userIdentity = new ApplicationUser {UserName = user.Customer.AnvandarNamn};

                //Skapar användaren i databasen
                var result = await _userManager.CreateAsync(userIdentity, user.Customer.Losenord);

                //Sätter rollen på nu användaren
                await _userManager.AddToRoleAsync(userIdentity, "Regular");


                //Om det går bra loggas användaren in
                if (result.Succeeded)
                {
                    _context.Kund.Add(user.Customer);

                    _context.SaveChanges();

                    //Sätter rollen på nu användaren
                    await _userManager.AddToRoleAsync(userIdentity, "Regular");
                    await _signInManager.SignInAsync(userIdentity, isPersistent: false);

                    return RedirectToAction("Start", "Home");
                }
            }


            return View();
        }

        
        public IActionResult AccountEdit()
        {
            var customer = _context.Kund.SingleOrDefault(x => x.AnvandarNamn == _userManager.GetUserName(User));
            var model = new TomasosModel()
            {
                Customer = customer
            };

            return View(model);
        }

        //Updaterar och skickar till bekräftelse VY. Laddar om tidigare vy om misslyckat
        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult EditConfirmation(TomasosModel newInfo)
        {
            if (ModelState.IsValid)
            {
                var customer = _context.Kund.SingleOrDefault(x => x.AnvandarNamn == _userManager.GetUserName(User));

                customer.Gatuadress = newInfo.Customer.Gatuadress;
                customer.Namn = newInfo.Customer.Namn;
                customer.Postnr = newInfo.Customer.Postnr;
                customer.Postort = newInfo.Customer.Postort;
                customer.Email = newInfo.Customer.Email;
                customer.Telefon = newInfo.Customer.Telefon;

                _context.SaveChanges();

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
            
            return RedirectToAction("AccountEdit", "Account");
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