using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TomasosASP.Models;

namespace TomasosASP.Controllers
{
    public class HomeController : Controller
    {
        private TomasosContext _context;

        public HomeController(TomasosContext context)
        {
            _context = context;
        }

        public IActionResult IndexLogoff()
        {
            var model = new ViewModels.LoginModel();

            return View(model);
        }

        public IActionResult IndexLogin(Kund customer)
        {


            return View(customer);
        }
        [HttpPost]
        public IActionResult ControlLogin(ViewModels.LoginModel login)
        {
            if (ModelState.IsValid)
            {
                var result = _context.Kund
                    .Where(x => x.AnvandarNamn == login.Username && x.Losenord == login.Password).ToList();
                    //_context.Kund.SingleOrDefault(x => x.AnvandarNamn == login.Username && x.Losenord == login.Password);

                var customer = result.First();


                if (customer == null)
                {
                    return RedirectToAction("LoginFail");
                }
                else
                {
                    return RedirectToAction("IndexLogin", customer);
                }

            }

            return RedirectToAction("LoginFail");
        }

        public IActionResult LoginFail()
        {
            var model = new ViewModels.LoginModel();

            return View(model);
        }
    }
}