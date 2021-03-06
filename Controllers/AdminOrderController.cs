﻿using System;
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
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly TomasosContext _context;

        //Dependency Injection via konstruktorn
        public AdminOrderController(
            ApplicationDbContext appContext,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
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
                Orders = _context.Bestallning.ToList(),
                Customers = _context.Kund.ToList(),
                BestallningMatratts = _context.BestallningMatratt.ToList()
            };

            return View(model);
        }

        public IActionResult Deliver(int id)
        {
            var order = _context.Bestallning.Single(x => x.BestallningId == id);
            order.Levererad = true;

            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var order = _context.Bestallning.Single(x => x.BestallningId == id);

            _context.BestallningMatratt.RemoveRange(_context.BestallningMatratt.Where(x => x.BestallningId == id));
            _context.Bestallning.Remove(_context.Bestallning.Single(x => x.BestallningId == id));

            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}