using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Pavestone.Data;
using Pavestone.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pavestone.Models.ViewModels;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.EntityFrameworkCore;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Authorization;

namespace Pavestone.Controllers
{
    [Authorize(Roles = Constants.AdminRole)]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> products = 
                _db.Products
                .Include(p => p.Category)
                .Include(p => p.ApplicationType);

            //foreach(var prod in products)
            //{
            //    prod.Category = _db.Categories.FirstOrDefault(c => c.Id == prod.CategoryId);
            //}

            return View(products);
        }

        public ActionResult Upsert(int? id)
        {
            IEnumerable<SelectListItem> categoryDropDown = _db.Categories
                .Select(c => new SelectListItem { Text = c.Name, Value = c.Id.ToString() });

            IEnumerable<SelectListItem> typeDropDown = _db.ApplicationTypes
                .Select(t => new SelectListItem { Text = t.Name, Value = t.Id.ToString() });

            //ViewBag.CategoryDropDown = categoryDropDown;

            //Product product = new Product();

            ProductVM productVM = new ProductVM
            { 
                Product = new Product(),
                CategorySelectList = categoryDropDown,
                ApplicationTypeSelectList = typeDropDown
            };

            if(id == null)
                return View(productVM);

            productVM.Product = _db.Products.Find(id);

            if (productVM.Product == null)
                return NotFound();

            return View(productVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Upsert(ProductVM productVM)
        {
            if (!ModelState.IsValid)
            {
                productVM.CategorySelectList = _db.Categories
                .Select(c => new SelectListItem { Text = c.Name, Value = c.Id.ToString() });
                productVM.ApplicationTypeSelectList = _db.ApplicationTypes
                .Select(t => new SelectListItem { Text = t.Name, Value = t.Id.ToString() });

                return View(productVM);
            }

            var files = HttpContext.Request.Form.Files;
            string webRootPath = _webHostEnvironment.WebRootPath;

            if(productVM.Product.Id == 0)
            {
                string uploadFolder = webRootPath + Constants.ProductImagesPath;
                string fileName = Guid.NewGuid().ToString();
                string extension = Path.GetExtension(files[0].FileName);

                using (var fileStream = new FileStream(Path.Combine(uploadFolder,fileName + extension), FileMode.Create))
                {
                    files[0].CopyTo(fileStream);
                }

                productVM.Product.Image = fileName + extension;
                _db.Products.Add(productVM.Product);
            }
            else
            {
                var prodFromDb = _db.Products.AsNoTracking().FirstOrDefault(p => p.Id == productVM.Product.Id);

                if(files.Count > 0)
                {
                    string uploadFolder = webRootPath + Constants.ProductImagesPath;
                    string fileName = Guid.NewGuid().ToString();
                    string extension = Path.GetExtension(files[0].FileName);

                    var oldFile = Path.Combine(uploadFolder, prodFromDb.Image);

                    if (System.IO.File.Exists(oldFile))
                    {
                        System.IO.File.Delete(oldFile);
                    }

                    using (var fileStream = new FileStream(Path.Combine(uploadFolder, fileName + extension), FileMode.Create))
                    {
                        files[0].CopyTo(fileStream);
                    }

                    productVM.Product.Image = fileName + extension;

                }
                else
                {
                    productVM.Product.Image = prodFromDb.Image;
                }

                _db.Products.Update(productVM.Product);
            }

            _db.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var product = _db.Products.Include(p => p.Category).Include(p => p.ApplicationType).FirstOrDefault(p => p.Id == id);

            if (product == null)
                return NotFound();

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            var product = _db.Products.Find(id);
            if (product == null)
                return NotFound();

            string webRootPath = _webHostEnvironment.WebRootPath;
            string uploadFolder = webRootPath + Constants.ProductImagesPath;
            string fileName = product.Image;
            var image = Path.Combine(uploadFolder, fileName);
            if (System.IO.File.Exists(image))
                System.IO.File.Delete(image);


            _db.Products.Remove(product);
            _db.SaveChanges();

            return RedirectToAction("Index");
        }


    }
}
