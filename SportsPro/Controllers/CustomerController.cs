using Microsoft.AspNetCore.Connections.Features;
using Microsoft.AspNetCore.Mvc;
using SportsPro.Models;
using System.Linq;

namespace SportsPro.Controllers
{
    public class CustomerController : Controller
    {
        private SportsProContext Context {  get; set; }

        public CustomerController(SportsProContext ctx) 
        {
            Context = ctx;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        [Route("/customers")]
        public IActionResult List()
        {
            var customers = Context.Customers.ToList();
            return View(customers);
        }
        [HttpGet]
        public IActionResult Add() 
        {
            ViewBag.Action = "Add";
            ViewBag.Countries = Context.Countries.OrderBy(c => c.Name).ToList();
            var customer = new Customer();
            return View("Edit", customer); 
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            ViewBag.Action = "Edit";
            var customer = Context.Customers.Find(id);
            ViewBag.Countries = Context.Countries.OrderBy(c => c.Name).ToList();
            return View(customer);
        }
        [HttpPost]
        public IActionResult Edit(Customer customer)
        {
            if (ModelState.IsValid)
            {
                if (customer.CustomerID == 0)
                {
                    Context.Customers.Add(customer);
                }
                else
                {
                    Context.Customers.Update(customer);
                }
                Context.SaveChanges();
                return RedirectToAction("List");
            }
            else
            {
                ViewBag.Action = (customer.CustomerID == 0) ? "Add" : "Edit";
                ViewBag.Countries = Context.Countries.OrderBy(c => c.Name).ToList();
                return View(customer);
            }

        }
        [HttpGet]
        public IActionResult Delete(int id)
        {
            ViewBag.Action = "Delete";
            var customer = Context.Customers.Find(id);
            return View(customer);
        }

        [HttpPost]
        public IActionResult Delete(Customer customer)
        {
            Context.Customers.Remove(customer);
            Context.SaveChanges();
            var customers = Context.Customers.ToList();
            return View("list", customers);
        }
    }
}
