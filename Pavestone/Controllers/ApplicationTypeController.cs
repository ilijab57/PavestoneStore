using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pavestone.Data;
using Pavestone.Models;
using Microsoft.AspNetCore.Authorization;

namespace Pavestone.Controllers
{
    [Authorize(Roles = Constants.AdminRole)]
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

        public IActionResult Edit(int id)
        {
            var type = _db.ApplicationTypes.Find(id);
            if (type == null)
                return NotFound();

            return View(type);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ApplicationType type)
        {
            if (!ModelState.IsValid)
                return View();

            _db.ApplicationTypes.Update(type);
            _db.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var type = _db.ApplicationTypes.Find(id);
            if (type == null)
                return NotFound();

            return View(type);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int id)
        {
            var type = _db.ApplicationTypes.Find(id);
            if (type == null)
                return NotFound();

            _db.ApplicationTypes.Remove(type);
            _db.SaveChanges();

            return RedirectToAction("Index");
        }


    }
}
