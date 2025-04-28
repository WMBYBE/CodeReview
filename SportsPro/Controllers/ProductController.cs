using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using SportsPro.Models;
using System.Linq;

namespace SportsPro.Controllers
{
    [Authorize(Roles = "Admin")]

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

        [Authorize(Roles = "Admin")]

        [HttpGet]
        public ViewResult Add()
        {
            // create new Product object
            Product product = new Product();                

            ViewBag.Action = "Add";
            // bind product to AddUpdate view
            return View("AddEdit", product);
        }
        [Authorize(Roles = "Admin")]

        [HttpGet]

        public ViewResult Edit(int id)
        {
            Product product = context.Products.FirstOrDefault(p => p.ProductID == id);
            ViewBag.Action = "Edit";

            return View("AddEdit", product);
        }
        [Authorize(Roles = "Admin")]

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
        [Authorize(Roles = "Admin")]

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var product = context.Products.FirstOrDefault(p => p.ProductID == id);

            var model = new DeleteConfirmationViewModel
            {
                ID = product.ProductID,
                Name = product.Name
            };
            return View(model);
        }
        [Authorize(Roles = "Admin")]


        [HttpPost]
        public RedirectToActionResult Delete(DeleteConfirmationViewModel model)
        {
            var product = context.Products.FirstOrDefault(p => p.ProductID == model.ID);
            context.Products.Remove(product);
            context.SaveChanges();
            return RedirectToAction("List");
        }
    }
}
