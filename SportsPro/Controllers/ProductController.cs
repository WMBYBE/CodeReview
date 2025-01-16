using Microsoft.AspNetCore.Mvc;
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
            var products = context.Products.OrderBy(c => c.ReleaseDate).ToList(); //Sends the lsit of forums to the index page so that you can see them all

            return View(products);
        }
        public IActionResult addProduct()
        {
            ViewBag.Action = "Add";
            return View("addProduct", new Product());
        }
        [HttpPost]
        public IActionResult addProduct(Product product)
        {
            if (ModelState.IsValid)
            {
                if (product.ProductCode == " ")
                    context.Products.Add(product);
                else
                    context.Products.Update(product);
                context.SaveChanges();
                return RedirectToAction("List", "Product");
            }
            else
            {
                ViewBag.Action = (product.ProductCode == " ") ? "Add" : "Edit";
                return View(product);
            }
        }
    }
}
