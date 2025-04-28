using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using SportsPro.Models;
using SportsPro.Models.datalayer;
using System.Linq;
using System.Security.Claims;

namespace SportsPro.Controllers
{

    public class ProductController : Controller
    {

        private Repository<Product> produt { get; set; }

        public ProductController(SportsProContext ctx)
        {
            produt = new Repository<Product>(ctx); 
        }


        [Authorize(Roles = "Admin,User")]

        public ViewResult List()
        {
            var prodOptions = new QueryOptions<Product>
            {
                OrderBy = d => d.ReleaseDate
            };

            //prodOptions.OrderBy = c => c.ReleaseDate;
            var list = produt.List(prodOptions);
            return View(list);
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
            Product product = produt.Get(id);

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
                    produt.Insert(product);
                }
                else
                {
                    TempData["updated"] = product.Name + " has been updated";
                    produt.Update(product);
                }

                var prodOptions = new QueryOptions<Product>
                {
                    OrderBy = d => d.ReleaseDate
                };

                produt.List(prodOptions);

                produt.Save();
                return RedirectToAction("List", prodOptions);
            }
            else
            {
                return RedirectToAction("AddEdit", product);
            }
        }
        [Authorize(Roles = "Admin")]

        [HttpGet]

        public ViewResult Delete(int id)
        {
            Product product = produt.Get(id);
            return View(product);
        }
        [Authorize(Roles = "Admin")]

        [HttpPost]

        public RedirectToActionResult Delete(Product product)
        {
            TempData["delete"] = product.Name + " has been deleted";
            produt.Delete(product);
            produt.Save();
            return RedirectToAction("List");
        }
    }
}
