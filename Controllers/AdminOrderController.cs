using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TomasosASP.Models;
using TomasosASP.ViewModels;

namespace TomasosASP.Views
{
    public class AdminOrderController : Controller
    {
        private readonly ApplicationDbContext _appContext;
        private readonly UserManager<User> _userManager;
        private readonly TomasosContext _context;

        //Dependency Injection via konstruktorn
        public AdminOrderController(
            ApplicationDbContext appContext,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            TomasosContext context
        )
        {
            _appContext = appContext;
            _userManager = userManager;
            _context = context;
        }

        public IActionResult Index()
        {
            var model = new AdminOrderView()
            {
                Orders = _context.Orders.ToList(),
                Customers = _context.Users.ToList(),
                BestallningMatratts = _context.OrderItems.ToList()
            };

            return View(model);
        }

        public IActionResult Deliver(int id)
        {
            var order = _context.Orders.Single(x => x.Id == id);
            order.Delivered = true;

            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var order = _context.Orders.Single(x => x.Id == id);

            _context.Orders.Remove(_context.Orders.Single(x => x.Id == id));

            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}