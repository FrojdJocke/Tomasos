using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TomasosASP.Models;

namespace TomasosASP.Controllers
{
    public partial class AccountController : Controller
    {
        private TomasosContext _context;

        public AccountController(TomasosContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult ControlLogin(ViewModels.LoginModel login)
        {
            if (ModelState.IsValid)
            {
                var customer = _context.Kund.SingleOrDefault(x => x.AnvandarNamn == login.Username && x.Losenord == login.Password);


                if (customer == null)
                {
                    return RedirectToAction("LoginFail","Home");
                }
                else
                {
                    return RedirectToAction("StartLogin", "LoggedIn", customer);
                }

            }

            return RedirectToAction("LoginFail","Home");
        }
    }
}
