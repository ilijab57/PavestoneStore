using Microsoft.AspNetCore.Mvc;
using Pavestone.Data;
using Pavestone.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pavestone.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CategoryController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            IEnumerable<Category> categories = _db.Categories;

            return View(categories);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Category category)
        {
            _db.Categories.Add(category);
            _db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
