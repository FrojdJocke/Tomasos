using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TomasosASP.Models;
using TomasosASP.ViewModels;

namespace TomasosASP.Controllers
{
    public class AdminProductController : Controller
    {
        private readonly ApplicationDbContext _appContext;
        private readonly UserManager<User> _userManager;
        private readonly TomasosContext _context;

        //Dependency Injection via konstruktorn
        public AdminProductController(
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
            HttpContext.Session.Clear();

            return View();
        }


        ///////////////////////////////////////////////////////////////////////////////////////////
        // New Product

        public IActionResult NewProduct()
        {
            HttpContext.Session.Clear();
            var types = _context.ProductCategories.Select(m => new SelectListItem
            {
                Value = m.Id.ToString(),
                Text = m.Description
            }).OrderBy(o => o.Text).ToList();

            List<Topping> ingredientList;
            if (HttpContext.Session.GetString("NewIngredients") == null)
            {
                ingredientList = new List<Topping>();
            }
            else
            {
                //Hämta listan från Sessionen
                var serializedValue = HttpContext.Session.GetString("NewIngredients");
                ingredientList = JsonConvert.DeserializeObject<List<Topping>>(serializedValue);
            }
            var temp = JsonConvert.SerializeObject(ingredientList);
            HttpContext.Session.SetString("NewIngredients", temp);

            var model = new AdminProductNewDish()
            {
                Ingredients = _context.Toppings.ToList(),
                Types = types,
                SelectedIngredients = ingredientList
            };

            //if (ingredientList.Count != 0)
            //{
            //    PartialView("_IngredientsPartial", ingredientList);
            //}
            return View(model);
        }


        public IActionResult AddIngredient(int? add, int? remove)
        {
            var newIngredient = _context.Toppings.SingleOrDefault(p => p.Id == add);
            var oldIngredient = _context.Toppings.SingleOrDefault(p => p.Id == remove);

            List<Topping> ingredientList;
            if (HttpContext.Session.GetString("NewIngredients") == null)
            {
                ingredientList = new List<Topping>();
            }
            else
            {
                //Hämta listan från Sessionen
                var serializedValue = HttpContext.Session.GetString("NewIngredients");
                ingredientList = JsonConvert.DeserializeObject<List<Topping>>(serializedValue);
            }
            if (add != null)
            {
                if (ingredientList.SingleOrDefault(i => i.Id == add) == null)
                {
                    ingredientList.Add(newIngredient);
                }
            }
            if (remove != null)
            {
                ingredientList.Remove(ingredientList.Single(x => x.Id == remove));
            }


            var temp = JsonConvert.SerializeObject(ingredientList);
            HttpContext.Session.SetString("NewIngredients", temp);

            return PartialView("_IngredientsPartial", ingredientList);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> SaveNewProduct(AdminProductNewDish product)
        {
            if (ModelState.IsValid)
            {
                List<Topping> ingredientList;
                if (HttpContext.Session.GetString("NewIngredients") == null)
                {
                    string error = "Inga ingridienser kopplade till maträtten. Försök igen";
                    return View("SaveProduct", error);
                }

                //Hämta listan från Sessionen
                var serializedValue = HttpContext.Session.GetString("NewIngredients");
                ingredientList = JsonConvert.DeserializeObject<List<Topping>>(serializedValue);


                var dishName = product.NewDish.Name;
                dishName = char.ToUpper(dishName[0]) + dishName.Substring(1);

                var newProduct = new Product()
                {
                    Name = dishName,
                    Description = product.NewDish.Description,
                    Category = new ProductCategory { Id = product.Type },
                    Price = product.NewDish.Price,
                    Toppings = ingredientList
                };

                _context.Products.Add(newProduct);

                _context.SaveChanges();

                return View("SaveProduct", "Ny produkt sparad!");
            }
            return RedirectToAction("NewProduct");
        }


        /////////////////////////////////////////////////////////////////////////////////////////////
        // Update Product

        public IActionResult ViewProducts()
        {
            return View(_context.Products.Include(x => x.OrderItems).ToList());
        }


        public IActionResult EditProduct(int id)
        {
            var product = _context.Products.Single(x => x.Id == id);

            var toppingsUsed = product.Toppings.Select(x => x.Id);                

            List<Topping> toppings = new List<Topping>();

            foreach (var topping in _context.Toppings.ToList())
            {
                if (!toppingsUsed.Contains(topping.Id))
                {
                    toppings.Add(topping);
                }
            }

            var model = new AdminProductEditDish()
            {
                Product = product,
                Toppings = _context.Toppings.ToList(),
            };

            return View(model);
        }


        public IActionResult Update(Product dish, int dishId, int? removeIngredient, int? addIngredient)
        {
            var product = _context.Products.FirstOrDefault(x => x.Id == dishId);
            //Remove Ingredient
            if (removeIngredient != null)
            {

                if(product != null && product.Toppings.Any(x => x.Id == removeIngredient))
                {
                    product.Toppings.Remove(product.Toppings.First(x => x.Id == removeIngredient));
                    _context.SaveChanges();
                }

                return RedirectToAction("EditProduct", new {id = dishId});
            }

            //AddIngredient
            if (addIngredient != null)
            {
                var topping = _context.Toppings.FirstOrDefault(x => x.Id == addIngredient);
                if (topping != null)
                    product.Toppings.Add(topping);

                _context.SaveChanges();

                return RedirectToAction("EditProduct", new {id = dishId});
            }

            //Save Dish
            var changeDish = _context.Products.Single(d => d.Id == dishId);

            dish.Name = char.ToUpper(dish.Name[0]) + dish.Name.Substring(1);

            changeDish.Name = dish.Name;
            changeDish.Description = dish.Description;
            changeDish.Price = dish.Price;

            _context.SaveChanges();

            return RedirectToAction("ViewProducts");
        }


        public IActionResult DeleteProduct(int id)
        {
            var product = _context.Products.Single(d => d.Id == id);

            _context.Remove(product);

            _context.SaveChanges();

            return RedirectToAction("ViewProducts");
        }

        /////////////////////////////////////////////////////////////////////////////////////////////
        // Add Ingredients

        public IActionResult NewIngredient()
        {
            var ingredients = _context.Toppings.ToList();
            var newIngredient = new Topping();

            var model = new AdminNewIngredient()
            {
                Ingredients = ingredients,
                NewIngredient = newIngredient
            };

            return View(model);
        }

        public IActionResult SaveIngredient(AdminNewIngredient value)
        {
            value.NewIngredient.Name = char.ToUpper(value.NewIngredient.Name[0]) +
                                              value.NewIngredient.Name.Substring(1);

            if (_context.Toppings.Any(x => x.Name == value.NewIngredient.Name))
            {
                return RedirectToAction("NewIngredient");
            }

            var newIngredient = new Topping()
            {
                Name = value.NewIngredient.Name
            };
            _context.Toppings.Add(newIngredient);
            _context.SaveChanges();

            return RedirectToAction("NewIngredient");
        }
    }
}