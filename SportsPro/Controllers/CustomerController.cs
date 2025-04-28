using Microsoft.AspNetCore.Connections.Features;
using Microsoft.AspNetCore.Mvc;
using SportsPro.Models;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Nodes;
using SportsPro.Models.datalayer;
using NuGet.DependencyResolver;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;


namespace SportsPro.Controllers
{

    public class CustomerController : Controller
    {
        //private SportsProContext Context {  get; set; }
        private Repository<Customer> Customer { get; set; }
        private Repository<Country> Country { get; set; }

        public CustomerController(SportsProContext ctx) 
        {
            Customer = new Repository<Customer>(ctx);
            Country = new Repository<Country>(ctx);
        }
        [Authorize(Roles = "Admin,User")]

        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Admin,User")]

        [HttpGet]
        [Route("/customers")]
        public IActionResult List()
        {
            var custOptions = new QueryOptions<Customer>
            {
                OrderBy = d => d.CustomerID
            };

            var customers = Customer.List(custOptions);
            return View(customers);
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Add() 
        {
            var contOptions = new QueryOptions<Country>
            {
                OrderBy = c => c.Name
            };

            ViewBag.Action = "Add";
            ViewBag.Countries = Country.List(contOptions);
            Customer c = new Customer();
            c.CountryID = "United States"; //DO NOT DELETE THIS LITERALLY FIXES EVERYTHING AND I DO NOT KNOW WHY. -Blade
            return View("Edit", c); 
        }
        [Authorize(Roles = "Admin")]

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var contOptions = new QueryOptions<Country>
            {
                OrderBy = c => c.Name
            };

            ViewBag.Action = "Edit";
            var customer = Customer.Get(id);
            ViewBag.Countries = Country.List(contOptions);
            return View(customer);
        }
        [Authorize(Roles = "Admin")]

        [HttpPost]
        public IActionResult Edit(Customer customer)
        {
            if (ModelState.IsValid)
            {
                if (customer.CustomerID == 0)
                {
                    Customer.Insert(customer);
                }
                else
                {
                    Customer.Update(customer);
                }
                Customer.Save();
                return RedirectToAction("List");
            }
            else
            {
                var contOptions = new QueryOptions<Country>
                {
                    OrderBy = c => c.Name
                };

                ViewBag.Action = (customer.CustomerID == 0) ? "Add" : "Edit";
                ViewBag.Countries = Country.List(contOptions);
                return View(customer);
            }

        }

        [Authorize(Roles = "Admin")]

        [HttpGet]
        public IActionResult Delete(int id)
        {
            ViewBag.Action = "Delete";
            var customer = Customer.Get(id);
            return View(customer);
        }
        [Authorize(Roles = "Admin")]

        [HttpPost]
        public IActionResult Delete(Customer customer)
        {
            var custOptions = new QueryOptions<Customer>
            {
                OrderBy = d => d.CustomerID
            };
            Customer.Delete(customer);
            Customer.Save();

            var customers = Customer.List(custOptions);
            return View("list", customers);
        }

        
        public JsonResult CheckEmail(string email)
        {
            var custOptions = new QueryOptions<Customer>
            {
                Where = c => c.Email == email
            };

            var hasEmail = Customer.Get(custOptions);
            if (hasEmail is null)
                return Json(true);
            else
                return Json($"Email address {email} is already registered.");
        }
    }
}
