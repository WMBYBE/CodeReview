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


namespace SportsPro.Controllers
{
    public class CustomerController : Controller
    {
<<<<<<< HEAD
        private IRepository<Customer> CustomerData { get; set; }
        private IRepository<Country> CountryData { get; set; }
=======
        //private SportsProContext Context {  get; set; }
        private Repository<Customer> Customer { get; set; }
        private Repository<Country> Country { get; set; }
>>>>>>> Blade-Branch


        public CustomerController(IRepository<Customer> Custrep, IRepository<Country> Counrep) 
        {
<<<<<<< HEAD
            CustomerData = Custrep;
            CountryData = Counrep;
=======
            Customer = new Repository<Customer>(ctx);
            Country = new Repository<Country>(ctx);
        }
        public IActionResult Index()
        {
            return View();
>>>>>>> Blade-Branch
        }
     
        [HttpGet]
        [Route("/customers")]
        public IActionResult List()
        {
<<<<<<< HEAD
            var customers = CustomerData.GetAll().ToList();
=======
            var custOptions = new QueryOptions<Customer>
            {
                OrderBy = d => d.CustomerID
            };

            var customers = Customer.List(custOptions);
>>>>>>> Blade-Branch
            return View(customers);
        }
        [HttpGet]
        public IActionResult Add() 
        {
            var contOptions = new QueryOptions<Country>
            {
                OrderBy = c => c.Name
            };

            ViewBag.Action = "Add";
<<<<<<< HEAD
            ViewBag.Countries = CountryData.GetAll().OrderBy(c => c.Name).ToList();
            var customer = new Customer();
            return View("Edit", customer); 
=======
            ViewBag.Countries = Country.List(contOptions);
            Customer c = new Customer();
            c.CountryID = "United States"; //DO NOT DELETE THIS LITERALLY FIXES EVERYTHING AND I DO NOT KNOW WHY. -Blade
            return View("Edit", c); 
>>>>>>> Blade-Branch
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var contOptions = new QueryOptions<Country>
            {
                OrderBy = c => c.Name
            };

            ViewBag.Action = "Edit";
<<<<<<< HEAD
            var customer = CustomerData.GetById(id);
            ViewBag.Countries = CountryData.GetAll().OrderBy(c => c.Name).ToList();
=======
            var customer = Customer.Get(id);
            ViewBag.Countries = Country.List(contOptions);
>>>>>>> Blade-Branch
            return View(customer);
        }
        [HttpPost]
        public IActionResult Edit(Customer customer)
        {
            if (ModelState.IsValid)
            {
                if (customer.CustomerID == 0)
                {
<<<<<<< HEAD
                    CustomerData.Add(customer);
                }
                else
                {
                    CustomerData.Update(customer);
                }
=======
                    Customer.Insert(customer);
                }
                else
                {
                    Customer.Update(customer);
                }
                Customer.Save();
>>>>>>> Blade-Branch
                return RedirectToAction("List");
            }
            else
            {
                var contOptions = new QueryOptions<Country>
                {
                    OrderBy = c => c.Name
                };

                ViewBag.Action = (customer.CustomerID == 0) ? "Add" : "Edit";
<<<<<<< HEAD
                ViewBag.Countries = CountryData.GetAll().OrderBy(c => c.Name).ToList();
=======
                ViewBag.Countries = Country.List(contOptions);
>>>>>>> Blade-Branch
                return View(customer);
            }

        }
        [HttpGet]
        public IActionResult Delete(int id)
        {
            ViewBag.Action = "Delete";
<<<<<<< HEAD
            var customer = CustomerData.GetById(id);
=======
            var customer = Customer.Get(id);
>>>>>>> Blade-Branch
            return View(customer);
        }

        [HttpPost]
        public IActionResult Delete(Customer customer)
        {
<<<<<<< HEAD
            CustomerData.Delete(customer.CustomerID);
            var customers = CustomerData.GetAll().ToList();
=======
            var custOptions = new QueryOptions<Customer>
            {
                OrderBy = d => d.CustomerID
            };
            Customer.Delete(customer);
            Customer.Save();

            var customers = Customer.List(custOptions);
>>>>>>> Blade-Branch
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
