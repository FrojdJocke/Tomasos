using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using TomasosASP.Models;
using TomasosASP.ViewModels;

namespace TomasosASP.Controllers
{
    public class AdminProductController : Controller
    {
        private readonly ApplicationDbContext _appContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly TomasosContext _context;

        //Dependency Injection via konstruktorn
        public AdminProductController(
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
            HttpContext.Session.Clear();

            return View();
        }

       
        ///////////////////////////////////////////////////////////////////////////////////////////
        // New Product
        
        public IActionResult NewProduct()
        {
            HttpContext.Session.Clear();
            var types = _context.MatrattTyp.Select(m => new SelectListItem
            {
                Value = m.MatrattTyp1.ToString(),
                Text = m.Beskrivning
            }).OrderBy(o => o.Text).ToList();


            var model = new AdminProductNewDish()
            {
                Ingredients = _context.Produkt.ToList(),
                Types = types
            };

            //if (ingredientList.Count != 0)
            //{
            //    PartialView("_IngredientsPartial", ingredientList);
            //}
            return View(model);
        }


        public IActionResult AddIngredient(int id)
        {
            var ingredient = _context.Produkt.SingleOrDefault(p => p.ProduktId == id);

            List<Produkt> ingredientList;
            if (HttpContext.Session.GetString("NewIngredients") == null)
            {
                ingredientList = new List<Produkt>();
            }
            else
            {
                //Hämta listan från Sessionen
                var serializedValue = HttpContext.Session.GetString("NewIngredients");
                ingredientList = JsonConvert.DeserializeObject<List<Produkt>>(serializedValue);
            }
            if (ingredientList.SingleOrDefault(i => i.ProduktId == id) == null)
            {
                ingredientList.Add(ingredient);
            }


            var temp = JsonConvert.SerializeObject(ingredientList);
            HttpContext.Session.SetString("NewIngredients", temp);

            return PartialView("_IngredientsPartial", ingredientList);
        }


        public IActionResult SaveNewProduct(AdminProductNewDish dish)
        {
            List<Produkt> ingredientList;
            if (HttpContext.Session.GetString("NewIngredients") == null)
            {
                string error = "Inga ingridienser kopplade till maträtten. Försök igen";
                return View("SaveProduct",error);
            }

            //Hämta listan från Sessionen
            var serializedValue = HttpContext.Session.GetString("NewIngredients");
            ingredientList = JsonConvert.DeserializeObject<List<Produkt>>(serializedValue);


            var dishName = dish.NewDish.MatrattNamn;
            dishName = char.ToUpper(dishName[0]) + dishName.Substring(1);

            var newProduct = new Matratt()
            {
                MatrattNamn = dishName,
                Beskrivning = dish.NewDish.Beskrivning,
                MatrattTyp = dish.Type,
                Pris = dish.NewDish.Pris
            };

            _context.Matratt.Add(newProduct);

            foreach (var item in ingredientList)
            {
                var newConn = new MatrattProdukt()
                {
                    MatrattId = newProduct.MatrattId,
                    ProduktId = item.ProduktId
                };
                _context.MatrattProdukt.Add(newConn);
            }
            _context.SaveChanges();

            return View("SaveProduct","Ny produkt sparad!");
        }


        /////////////////////////////////////////////////////////////////////////////////////////////
        // Update Product

        public IActionResult ViewProducts()
        {
            return View(_context.Matratt.ToList());
        }
        

        public IActionResult EditProduct(int id)
        {
            var dish = _context.Matratt.Single(x => x.MatrattId == id);

            var dishConnUsed = _context.MatrattProdukt.Where(x => x.MatrattId == dish.MatrattId).Select(x => x.ProduktId).ToList();

            List<Produkt> prods = new List<Produkt>();

            foreach (var prod in _context.Produkt.ToList())
            {
                if (!dishConnUsed.Contains(prod.ProduktId))
                {
                    prods.Add(prod);
                }
            }
            
            var model = new AdminProductEditDish()
            {
                Dish = dish,
                DishIngredients = _context.MatrattProdukt.ToList(),
                Ingredients = _context.Produkt.ToList(),
                NotUsedIngredients = prods
            };

            return View(model);
        }
        

        public IActionResult Update(Matratt dish, int? dishId, int? removeIngredient, int? addIngredient)
        {
            //Remove Ingredient
            if (removeIngredient != null)
            {
                _context.MatrattProdukt.Remove(_context.MatrattProdukt.SingleOrDefault(x =>
                    x.MatrattId == dishId && x.ProduktId == removeIngredient));

                _context.SaveChanges();

                return RedirectToAction("EditProduct", new { id = dishId});
            }

            //AddIngredient
            if (addIngredient != null && dishId != null)
            {
                var newValue = new MatrattProdukt()
                {
                    MatrattId = (int)dishId,
                    ProduktId = (int)addIngredient
                };

                _context.MatrattProdukt.Add(newValue);

                _context.SaveChanges();

                return RedirectToAction("EditProduct", new { id = dishId });
            }

            //Save Dish
            var changeDish = _context.Matratt.Single(d => d.MatrattId == dishId);

            dish.MatrattNamn = char.ToUpper(dish.MatrattNamn[0]) + dish.MatrattNamn.Substring(1);

            changeDish.MatrattNamn = dish.MatrattNamn;
            changeDish.Beskrivning = dish.Beskrivning;
            changeDish.Pris = dish.Pris;

            _context.SaveChanges();

            return RedirectToAction("ViewProducts");
        }


        public IActionResult DeleteProduct(int id)
        {
            var dish = _context.Matratt.Single(d => d.MatrattId == id);

            _context.MatrattProdukt.RemoveRange(_context.MatrattProdukt.Where(x => x.MatrattId == id));
            _context.Remove(dish);

            _context.SaveChanges();

            return RedirectToAction("ViewProducts");
        }

        /////////////////////////////////////////////////////////////////////////////////////////////
        // Add Ingredients

        public IActionResult NewIngredient()
        {
            var ingredients = _context.Produkt.ToList();
            var newIngredient = new Produkt();

            var model = new AdminNewIngredient()
            {
                Ingredients = ingredients,
                NewIngredient = newIngredient
            };

            return View(model);
        }

        public IActionResult SaveIngredient(AdminNewIngredient value)
        { 

            value.NewIngredient.ProduktNamn = char.ToUpper(value.NewIngredient.ProduktNamn[0]) + value.NewIngredient.ProduktNamn.Substring(1);

            if (_context.Produkt.Any(x => x.ProduktNamn == value.NewIngredient.ProduktNamn))
            {
                return RedirectToAction("NewIngredient");
            }
            
            var newIngredient = new Produkt()
            {
                ProduktNamn = value.NewIngredient.ProduktNamn
            };
            _context.Produkt.Add(newIngredient);
            _context.SaveChanges();

            return RedirectToAction("NewIngredient");
        }
    }
}