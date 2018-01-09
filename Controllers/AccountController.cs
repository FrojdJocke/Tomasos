using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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


        

        //public AccountController(TomasosContext context)
        //{
        //    _context = context;
        //}

        
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public IActionResult ControlLogin(ViewModels.LoginModel login)
        {
            if (ModelState.IsValid)
            {
                var customer = _context.Kund.SingleOrDefault(x => x.AnvandarNamn == login.Username && x.Losenord == login.Password);


                if (customer == null)
                {
                    return RedirectToAction("Login","Home");
                }

                
                return RedirectToAction("StartLogin", "LoggedIn", customer);
                

            }
            else {return RedirectToAction("Login", "Home"); }
            

        }

        //En inloggningsmetod måste alltid tillåta anonym access
        //[AllowAnonymous]

        //public IActionResult Login()
        //{
        //    return View();
        //}

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(LoginModel user)
        {
            var model = new ViewModels.LoginModel();
            if (ModelState.IsValid)
            {
                var customer = _context.Kund.SingleOrDefault(x => x.AnvandarNamn == user.Username && x.Losenord == user.Password);


                var result = await _signInManager.PasswordSignInAsync(user.Username, user.Password, true, false);

                if (result.Succeeded)
                {
                    //Om inloggningen gick bra visas startsidan
                    return RedirectToAction("StartLogin", "LoggedIn", customer);
                }
                return View(model);
            }


            return View(model);

        }

        public async Task<IActionResult> LogOff()
        { 
            await _signInManager.SignOutAsync();

            return RedirectToAction("Start", "Home");
            
        }

        [AllowAnonymous]
        public IActionResult Register()
        {
            var model = new Kund();
            return View(model);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register(Kund user)
        {

            if (_context.Kund.SingleOrDefault(x => x.AnvandarNamn == user.AnvandarNamn) != null)
            {
                return View();
            }
            //Lägger över värdena från sidan i en ApplicationUser klass
            var userIdentity = new ApplicationUser { UserName = user.AnvandarNamn };

            

            //Skapar användaren i databasen
            var result = await _userManager.CreateAsync(userIdentity, user.Losenord);

            //Sätter rollen på nu användaren
            await _userManager.AddToRoleAsync(userIdentity, "Regular");


            //Om det går bra loggas användaren in
            if (result.Succeeded)
            {
                _context.Kund.Add(user);

                _context.SaveChanges();

                //Sätter rollen på nu användaren
                await _userManager.AddToRoleAsync(userIdentity, "Regular");
                await _signInManager.SignInAsync(userIdentity, isPersistent: false);

                return RedirectToAction("StartLogin", "LoggedIn", user);
            }

            return View();
        }

    }
}

