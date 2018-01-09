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

        public IActionResult Start()
        {
            
            var model = new ViewModels.LoginModel();

            return View(model);
        }
        

        public IActionResult Login()
        {
            var model = new ViewModels.LoginModel();

            return View(model);
        }
    }
}