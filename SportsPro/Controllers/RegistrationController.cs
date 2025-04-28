using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportsPro.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SportsPro.Controllers
{
    public class RegistrationController : Controller
    {
        private SportsProContext context { get; set; }
        private List<Customer> customers;
        private List<Product> products;


        public RegistrationController(SportsProContext ctx)
        {

            context = ctx;
            customers = context.Customers
                    .OrderBy(c => c.CustomerID)
                    .ToList();
            products = context.Products
                    .OrderBy(c => c.ProductID)
                    .ToList();
        }
        [HttpGet]
        public ActionResult GetCustomer()
        {

            ViewBag.Customers = context.Customers.ToList();
            return View();

        }
        [HttpPost]
        public ActionResult GetCustomer(int? CustomerID)
        {
            if (CustomerID ==null)
            {
                ViewBag.ErrorMessage = "Please Select a customer,";
                ViewBag.Customers = context.Customers.ToList();
                return View();
            }

            return RedirectToAction(actionName: "Index", new { customerID = CustomerID.Value });
        }

        [HttpGet]
        public ActionResult Index (int CustomerID)
        {
            var customer = context.Customers
                .Include(c => c.CustomerProducts)
                .ThenInclude(cp => cp.Product)
                .FirstOrDefault(c => c.CustomerID == CustomerID);

            if (customer == null)
            {
                TempData["Message"] = "Customer not found";
                return RedirectToAction("Index", "Home");
            }

            var allProducts = context.Products.ToList();
            var registeredProductIds = customer.CustomerProducts
                .Select(c => c.ProductID)
                .ToList();
            var unregisteredProducts = allProducts
                .Where(p => !registeredProductIds
                .Contains(p.ProductID))
                .ToList();

            var viewModel = new RegistrationViewModel
            {
                Customer = customer,
                RegisteredProducts = customer.CustomerProducts.Select(cp => cp.Product).ToList(),
                AvailableProducts = unregisteredProducts
            };

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult RegisterProduct(int customerID, int productID)
        {
            var validRegistration = context.CustomerProducts
                .FirstOrDefault(cp => cp.CustomerID == customerID && cp.ProductID == productID);

            if(validRegistration != null)
            {
                return RedirectToAction("Index", new { customerID });
            }

            var registration = new CustomerProduct
                {
                    CustomerID = customerID,
                    ProductID = productID,
                    RegistrationDate = DateTime.Now
            };

            context.CustomerProducts.Add(registration);
            context.SaveChanges();

            return RedirectToAction("Index", new { customerID });
        }

        [HttpPost]
        public ActionResult Delete(int customerID, int productID)
        {
            var customerProduct = context.CustomerProducts
                .FirstOrDefault(cp => cp.CustomerID == customerID && cp.ProductID == productID);

            context.CustomerProducts.Remove(customerProduct);
            context.SaveChanges();

            var customer = context.Customers
                 .Include(c => c.CustomerProducts)
                 .ThenInclude(cp => cp.Product)
                 .FirstOrDefault(c => c.CustomerID == customerID);

            if (customer == null)
            {
                TempData["Message"] = "Customer not found";
                return RedirectToAction("Index", "Home");
            }

            var allProducts = context.Products.ToList();
            var registeredProductIds = customer.CustomerProducts
                .Select(c => c.ProductID)
                .ToList();
            var unregisteredProducts = allProducts
                .Where(p => !registeredProductIds
                .Contains(p.ProductID))
                .ToList();

            var vm = new RegistrationViewModel
            {
                Customer = customer,
                RegisteredProducts = customer.CustomerProducts.Select(cp => cp.Product).ToList(),
                AvailableProducts = unregisteredProducts
            };

            return View("Index", vm);
        }
    }
    
}
