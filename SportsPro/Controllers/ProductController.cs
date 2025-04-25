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
<<<<<<< HEAD
        private IRepository<Product> data { get; set; }
=======

        private Repository<Product> produt { get; set; }
>>>>>>> Blade-Branch


        public ProductController(IRepository<Product> rep)
        {
<<<<<<< HEAD
            data = rep;
=======
            produt = new Repository<Product>(ctx); 
>>>>>>> Blade-Branch
        }



        public ViewResult List()
        {
<<<<<<< HEAD
            var products = data.GetAll().OrderBy(p => p.ReleaseDate).ToList();
            return View(products);
=======
            var prodOptions = new QueryOptions<Product>
            {
                OrderBy = d => d.ReleaseDate
            };

            //prodOptions.OrderBy = c => c.ReleaseDate;
            var list = produt.List(prodOptions);
            return View(list);
>>>>>>> Blade-Branch
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
<<<<<<< HEAD
            Product product = data.GetById(id);
=======
            Product product = produt.Get(id);

>>>>>>> Blade-Branch
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
<<<<<<< HEAD
                    data.Add(product);
=======
                    produt.Insert(product);
>>>>>>> Blade-Branch
                }
                else
                {
                    TempData["updated"] = product.Name + " has been updated";
<<<<<<< HEAD
                    data.Update(product);
                }

                return RedirectToAction("List");

            } 
=======
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
>>>>>>> Blade-Branch
            else
            {
                return RedirectToAction("AddEdit", product);
            }
        }
        [HttpGet]

        public ViewResult Delete(int id)
        {
<<<<<<< HEAD
            Product product = data.GetById(id);
=======
            Product product = produt.Get(id);
>>>>>>> Blade-Branch
            return View(product);
        }
        [HttpPost]

        public RedirectToActionResult Delete(Product product)
        {
            TempData["delete"] = product.Name + " has been deleted";
<<<<<<< HEAD
            data.Delete(product.ProductID);
=======
            produt.Delete(product);
            produt.Save();
>>>>>>> Blade-Branch
            return RedirectToAction("List");
        }
    }
}
