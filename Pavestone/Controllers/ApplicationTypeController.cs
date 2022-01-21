using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pavestone.Data;
using Pavestone.Models;

namespace Pavestone.Controllers
{
    public class ApplicationTypeController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ApplicationTypeController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            IEnumerable<ApplicationType> types = _db.ApplicationTypes;
            return View(types);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Create(ApplicationType type)
        {
            if (!ModelState.IsValid)
                return View();

            _db.ApplicationTypes.Add(type);
            _db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
