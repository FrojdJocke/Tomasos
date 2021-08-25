using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TomasosASP.Models;
using TomasosASP.ViewModels;

namespace TomasosASP.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly TomasosContext _context;

        //Dependency Injection via konstruktorn
        public OrderController
        (
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            TomasosContext context
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        [Route("Meny")]
        public IActionResult Menu()
        {
            var dish = _context.Products.ToList();
            var ingredients = _context.Toppings.ToList();
            var type = _context.ProductCategories.ToList();
            var customer = _context.Users.SingleOrDefault(x => x.UserName == _userManager.GetUserName(User));
            List<OrderItem> prodList;
            if (HttpContext.Session.GetString("Varukorg") == null)
            {
                prodList = null;
            }
            else
            {
                var serializedValue = HttpContext.Session.GetString("Varukorg");
                prodList = JsonConvert.DeserializeObject<List<OrderItem>>(serializedValue);
            }

            int numberOfItems = 0;
            if (prodList == null)
            {
                numberOfItems = 0;
            }
            else
            {
                foreach (var item in prodList)
                {
                    numberOfItems += item.Quantity;
                }
            }

            var model = new TomasosModel
            {
                Customer = customer,
                Products = dish,
                Ingredients = ingredients,
                ProductCatories = type,
                itemsInCart = numberOfItems,
                Cart = prodList
            };
            return View(model);
        }

        public IActionResult AddProduct(int dishID)
        {
            //Här läggs produkten till i varukorgen
            var product = _context.Products.SingleOrDefault(m => m.Id == dishID);

            List<OrderItem> cart;

            if (HttpContext.Session.GetString("Varukorg") == null)
            {
                cart = new List<OrderItem>();
            }
            else
            {
                //Hämta listan från Sessionen
                var serializedValue = HttpContext.Session.GetString("Varukorg");
                cart = JsonConvert.DeserializeObject<List<OrderItem>>(serializedValue);
            }
            if (cart.Select(x => x.Product).Any(x => x.Id == dishID))
            {
                cart.First(x => x.Product.Id == dishID).Quantity++;
            }
            else
            {
                var addMore = new OrderItem()
                {
                    Quantity = 1,
                    Amount = product.Price,
                    Product = product
                };

                cart.Add(addMore);
            }

            int numberOfItems = 0;
            if (cart == null)
            {
                numberOfItems = 0;
            }
            else
            {
                foreach (var item in cart)
                {
                    numberOfItems += item.Quantity;
                }
            }

            var temp = JsonConvert.SerializeObject(cart);
            HttpContext.Session.SetString("Varukorg", temp);


            return PartialView("_CartPartial", numberOfItems);
        }
                
        public IActionResult RemoveProduct(int dishID)
        {
            //Här läggs produkten till i varukorgen
            var product = _context.Products.SingleOrDefault(m => m.Id == dishID);

            List<OrderItem> cart;

            //Hämta listan från Sessionen
            var serializedValue = HttpContext.Session.GetString("Varukorg");
            cart = JsonConvert.DeserializeObject<List<OrderItem>>(serializedValue);

            cart.Remove(cart.SingleOrDefault(x => x.Product.Id == dishID));

            var temp = JsonConvert.SerializeObject(cart);
            HttpContext.Session.SetString("Varukorg", temp);

            return RedirectToAction("Checkout");
        }

        [Route("Kassan")]
        public IActionResult Checkout()
        {
            List<OrderItem> cart;
            string serializedValue;
            if (HttpContext.Session.GetString("Varukorg") == null)
            {
                cart = new List<OrderItem>();
            }
            else
            {
                //Hämta listan från Sessionen
                serializedValue = HttpContext.Session.GetString("Varukorg");
                cart = JsonConvert.DeserializeObject<List<OrderItem>>(serializedValue);
            }

            serializedValue = HttpContext.Session.GetString("Varukorg");

            try
            {
                cart = JsonConvert.DeserializeObject<List<OrderItem>>(serializedValue);
            }
            catch
            {
                cart = new List<OrderItem>();
            }

            int sum = cart.Sum(x => x.Product.Price * x.Quantity);

            var temp = JsonConvert.SerializeObject(cart);
            HttpContext.Session.SetString("Varukorg", temp);

            var model = GetCustomer(cart);
            model.TotalSum = sum;

            if (model.Customer.Points >= 100 && User.IsInRole("Premium"))
            {
                if (cart.Count != 0)
                {
                    int discount = model.Cart.Max(x => x.Product.Price);
                    model.Discount = discount;
                }
            }


            return View(model);
        }


        public IActionResult ConfirmOrder()
        {
            var serializedValue = HttpContext.Session.GetString("Varukorg");
            var cart = JsonConvert.DeserializeObject<List<OrderItem>>(serializedValue);
            int sum = cart.Sum(x => x.Product.Price * x.Quantity);
            int points = cart.Sum(x => x.Quantity * 10);
            double discount = 1;
            if (cart.Sum(x => x.Quantity) >= 3) discount = 0.8;
            var customer = _context.Users.SingleOrDefault(k => k.UserName == _userManager.GetUserName(User));
            Order newOrder;

            if (User.IsInRole("Premium"))
            {
                if (customer.Points >= 100)
                {
                    double freeDish = cart.Max(x => x.Product.Price);
                    newOrder = new Order()
                    {
                        OrderDate = DateTime.Now,
                        TotalAmount = (int) ((sum * discount) - (freeDish * discount)),
                        Delivered = false,
                        Customer = customer
                    };
                    customer.Points -= 100;
                }

                else
                {
                    newOrder = new Order()
                    {
                        OrderDate = DateTime.Now,
                        TotalAmount = (int) (sum * discount),
                        Delivered = false,
                        Customer = customer
                    };
                }
                customer.Points += points;
            }

            else
            {
                newOrder = new Order()
                {
                    OrderDate = DateTime.Now,
                    TotalAmount = (int) sum,
                    Delivered = false,
                    Customer = customer
                };
            }

            newOrder.Items = cart;
            _context.Orders.Add(newOrder);

            //_context.SaveChanges();

            //foreach (var item in cart)
            //{
            //    var orderItem = new OrderItem()
            //    {
            //        Order = newOrder,
            //        Product = item.Product,
            //        Quantity = item.Quantity
            //    };
            //    _context.OrderItems.Add(orderItem);
            //}

            _context.SaveChanges();
            HttpContext.Session.Clear();


            return RedirectToAction("Menu");
        }


        public TomasosModel GetCustomer(List<OrderItem> prodList)
        {
            int numberOfItems = 0;
            if (prodList == null)
            {
                numberOfItems = 0;
            }
            else
            {
                foreach (var item in prodList)
                {
                    numberOfItems += item.Quantity;
                }
            }


            var model = new TomasosModel
            {
                Customer = _context.Users.SingleOrDefault(x => x.UserName == _userManager.GetUserName(User)),
                Products = _context.Products.ToList(),
                Ingredients = _context.Toppings.ToList(),
                ProductCatories = _context.ProductCategories.ToList(),
                itemsInCart = numberOfItems,
                Cart = prodList
            };

            return model;
        }
    }
}