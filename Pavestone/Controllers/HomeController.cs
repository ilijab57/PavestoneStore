using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pavestone.Data;
using Pavestone.Models;
using Pavestone.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Pavestone.Utility;

namespace Pavestone.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        public IActionResult Index()
        {
            HomeVM homeVM = new HomeVM() 
            { 
                Products = _db.Products.Include(p => p.Category).Include(p => p.ApplicationType),
                Categories = _db.Categories
            };
            return View(homeVM);
        }


        public IActionResult Details(int id)
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(Constants.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(Constants.SessionCart).Count() > 0)
            {
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(Constants.SessionCart);
            }

            DetailsVM detailsVM = new DetailsVM
            {
                Product = _db.Products.Include(p => p.Category)
                                      .Include(p => p.ApplicationType)
                                      .Where(p => p.Id == id)
                                      .FirstOrDefault(),
                ExistsInCart = false
            };

            foreach(var item in shoppingCartList)
            {
                if(item.ProductId == id)
                {
                    detailsVM.ExistsInCart = true;
                }
            }

            return View(detailsVM);
        }


        [HttpPost, ActionName("Details")]
        public IActionResult DetailsPost(int id)
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(Constants.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(Constants.SessionCart).Count() > 0)
            {
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(Constants.SessionCart);
            }

            shoppingCartList.Add(new ShoppingCart { ProductId = id });
            HttpContext.Session.Set(Constants.SessionCart, shoppingCartList);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult RemoveFromCart(int id)
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(Constants.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(Constants.SessionCart).Count() > 0)
            {
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(Constants.SessionCart);
            }

            var itemToRemove = shoppingCartList.SingleOrDefault(i => i.ProductId == id);
            if(itemToRemove != null)
            {
                shoppingCartList.Remove(itemToRemove);
            }

            HttpContext.Session.Set(Constants.SessionCart, shoppingCartList);

            return RedirectToAction(nameof(Index));

        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
