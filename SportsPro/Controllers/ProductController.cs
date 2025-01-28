using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using SportsPro.Models;
using System.Linq;

namespace SportsPro.Controllers
{
    public class ProductController : Controller
    {
        private SportsProContext context { get; set; }

        public ProductController(SportsProContext ctx)
        {
            context = ctx;
        }
        public IActionResult List()
        {
            var products = context.Products.OrderBy(c => c.ReleaseDate).ToList();
            return View(products);
        }

        [HttpGet]
        public IActionResult Add()
        {
            // create new Product object
            Product product = new Product();                

            ViewBag.Action = "Add";

            // bind product to AddEdit view
            return View("AddEdit", product);
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            Product product = context.Products.FirstOrDefault(p => p.ProductID == id);
            ViewBag.Action = "Edit";

            return View("AddEdit", product);
        }
        [HttpPost]
        public IActionResult Edit(Product product)
        {
            if (ModelState.IsValid)
            {
                if (product.ProductID == 0)
                {
                    context.Products.Add(product);
                }
                else
                {
                    context.Products.Update(product);
                }
                context.SaveChanges();
                return RedirectToAction("List");
            }
            else
            {
                ViewBag.Action = "Save";
                return View("AddEdit", product);
            }
        }
        [HttpGet]
        public IActionResult Delete(int id)
        {
            Product product = context.Products
                .FirstOrDefault(p => p.ProductID == id);
            return View(product);
        }

        [HttpPost]
        public IActionResult Delete(Product product)
        {
            context.Products.Remove(product);
            context.SaveChanges();
            return RedirectToAction("List");
        }
    }
}
