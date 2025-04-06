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
        public ViewResult List()
        {
            var products = context.Products.OrderBy(c => c.ReleaseDate).ToList();
            return View(products);
        }

        [HttpGet]
        public ViewResult Add()
        {
            // create new Product object
            Product product = new Product();                

            ViewBag.Action = "Add";
            // bind product to AddUpdate view
            return View("AddEdit", product);
        }
        [HttpGet]

        public ViewResult Edit(int id)
        {
            Product product = context.Products.FirstOrDefault(p => p.ProductID == id);
            ViewBag.Action = "Edit";

            return View("AddEdit", product);
        }
        [HttpPost]

        public RedirectToActionResult Edit(Product product)
        {
            if (ModelState.IsValid)
            {
                if (product.ProductID == 0)
                {
                    TempData["add"] = product.Name + " has been added";
                    context.Products.Add(product);
                }
                else
                {
                    TempData["updated"] = product.Name + " has been updated";
                    context.Products.Update(product);
                }

                var products = context.Products.OrderBy(c => c.ReleaseDate).ToList();
                context.SaveChanges();
                return RedirectToAction("List", products);
            }
            else
            {
                return RedirectToAction("AddEdit", product);
            }
        }
        [HttpGet]

        public ViewResult Delete(int id)
        {
            Product product = context.Products.FirstOrDefault(p => p.ProductID == id);
            return View(product);
        }
        [HttpPost]

        public RedirectToActionResult Delete(Product product)
        {
            TempData["delete"] = product.Name + " has been deleted";
            context.Products.Remove(product);
            context.SaveChanges();
            return RedirectToAction("List");
        }
    }
}
