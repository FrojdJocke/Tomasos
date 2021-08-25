using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using TomasosASP.Models;
using TomasosASP.ViewModels;

namespace TomasosASP.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Test()
        {
            var model = new Product();

            return View(model);
        }
        
    }
}