using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TomasosASP.Models;
using TomasosASP.ViewModels;

namespace TomasosASP.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminUserController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly TomasosContext _context;

        //Dependency Injection via konstruktorn
        public AdminUserController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            TomasosContext context
        )
        {
            _userManager = userManager;
            _context = context;
        }

        public IActionResult Index()
        {
            var model = new List<AdminUserIndex>();
            var users = _context.Users.OrderBy(u => u.UserName);

            foreach (var item in users)
            {
                if (item.UserName == _userManager.GetUserName(User))
                {
                    continue;
                }
                var userRoles = _userManager.GetRolesAsync(item).Result;

                var role = userRoles.Count == 0 ? "Not set" : userRoles[0];

                model.Add(new AdminUserIndex()
                {
                    User = item,
                    Role = role,
                });
            }

            return View(model);
        }

        public IActionResult EditRole(string user)
        {
            var getUser = _context.Users.Single(x => x.UserName == user);

            var userRoles = _userManager.GetRolesAsync(getUser).Result;

            var role = userRoles.Count == 0 ? "Not set" : userRoles[0];

            var roles = _context.Roles.Distinct().OrderBy(x => x.Name).Select(p => new SelectListItem()
            {
                Value = p.Id,
                Text = p.Name
            }).OrderBy(o => o.Text).ToList();
            

            var model = new AdminUserEditRole()
            {
                Roles = roles,
                Role = role,
                User = getUser
            };

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRole(AdminUserEditRole vm)
        {
            if (ModelState.IsValid)
            {
                var user = _context.Users.Single(x => x.UserName == vm.User.UserName);

                // Get roles
                var roles = await _userManager.GetRolesAsync(user);


                // Remove roles
                foreach (var item in roles)
                {
                    await _userManager.RemoveFromRoleAsync(user, item);
                }

                var role = _context.Roles.Find(vm.SelectedRole);

                // Set new role
                await _userManager.AddToRoleAsync(user, role.Name);

                return RedirectToAction("Index");
            }
            return View();
        }


        public async Task<IActionResult> DeleteUser(string userName)
        {
            var tmp = await _userManager.FindByNameAsync(userName);
            await _userManager.DeleteAsync(tmp);

            return RedirectToAction("Index");
        }
    }
}