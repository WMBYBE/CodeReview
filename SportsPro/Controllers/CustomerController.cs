using Microsoft.AspNetCore.Connections.Features;
using Microsoft.AspNetCore.Mvc;
using SportsPro.Models;
using System.Linq;

namespace SportsPro.Controllers
{
    public class CustomerController : Controller
    {
        private IRepository<Customer> CustomerData { get; set; }
        private IRepository<Country> CountryData { get; set; }


        public CustomerController(IRepository<Customer> Custrep, IRepository<Country> Counrep) 
        {
            CustomerData = Custrep;
            CountryData = Counrep;
        }
     
        [HttpGet]
        [Route("/customers")]
        public IActionResult List()
        {
            var customers = CustomerData.GetAll().ToList();
            return View(customers);
        }
        [HttpGet]
        public IActionResult Add() 
        {
            ViewBag.Action = "Add";
            ViewBag.Countries = CountryData.GetAll().OrderBy(c => c.Name).ToList();
            var customer = new Customer();
            return View("Edit", customer); 
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            ViewBag.Action = "Edit";
            var customer = CustomerData.GetById(id);
            ViewBag.Countries = CountryData.GetAll().OrderBy(c => c.Name).ToList();
            return View(customer);
        }
        [HttpPost]
        public IActionResult Edit(Customer customer)
        {
            if (ModelState.IsValid)
            {
                if (customer.CustomerID == 0)
                {
                    CustomerData.Add(customer);
                }
                else
                {
                    CustomerData.Update(customer);
                }
                return RedirectToAction("List");
            }
            else
            {
                ViewBag.Action = (customer.CustomerID == 0) ? "Add" : "Edit";
                ViewBag.Countries = CountryData.GetAll().OrderBy(c => c.Name).ToList();
                return View(customer);
            }

        }
        [HttpGet]
        public IActionResult Delete(int id)
        {
            ViewBag.Action = "Delete";
            var customer = CustomerData.GetById(id);
            return View(customer);
        }

        [HttpPost]
        public IActionResult Delete(Customer customer)
        {
            CustomerData.Delete(customer.CustomerID);
            var customers = CustomerData.GetAll().ToList();
            return View("list", customers);
        }
    }
}
