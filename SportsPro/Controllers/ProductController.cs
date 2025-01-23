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
        public IActionResult addProduct()
        {
            // create new Product object
            Product product = new Product();                

            ViewBag.Action = "Add";

            // bind product to AddUpdate view
            return View("addProduct", product);
        }

        public IActionResult edit(int id)
        {
            Product product = context.Products.FirstOrDefault(p => p.ProductID == id);
            ViewBag.Action = "Edit";

            return View("addProduct", new Product());
        }
        [HttpPost]
        public IActionResult addProduct(Product product)
        {
            if (ModelState.IsValid)
            {
                if (product.ProductID == 0)
                    context.Products.Add(product);
                else
                    context.Products.Update(product);
                context.SaveChanges();
                return RedirectToAction("List", "Product");
            }
            else
            {
                ViewBag.Action = (product.ProductID == 0) ? "Add" : "Edit";
                return View(product);
            }
        }
    }
}
