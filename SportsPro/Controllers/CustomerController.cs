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
        private SportsProContext Context {  get; set; }

        public CustomerController(SportsProContext ctx) 
        {
            Customer = new Repository<Customer>(ctx);
            Country = new Repository<Country>(ctx);
        }
        [Authorize(Roles = "Admin")]

        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]

        [HttpGet]
        [Route("/customers")]
        public IActionResult List()
        {
            var customers = Context.Customers.ToList();
            return View(customers);
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Add() 
        {
            ViewBag.Action = "Add";
            ViewBag.Countries = Context.Countries.OrderBy(c => c.Name).ToList();
            var customer = new Customer();
            return View("Edit", customer); 
        }
        [Authorize(Roles = "Admin")]

        [HttpGet]
        public IActionResult Edit(int id)
        {
            ViewBag.Action = "Edit";
            var customer = Context.Customers.Find(id);
            ViewBag.Countries = Context.Countries.OrderBy(c => c.Name).ToList();
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

        [Authorize(Roles = "Admin")]

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var customer = Context.Customers.Find(id);

            var model = new DeleteConfirmationViewModel
            {
                ID = customer.CustomerID,
                Name = customer.FullName
            };
            return View(model);
        }
        [Authorize(Roles = "Admin")]

        [HttpPost]
        public IActionResult Delete(DeleteConfirmationViewModel model)
        {
            var customer = Context.Customers.Find(model.ID);
            Context.Customers.Remove(customer);
            Context.SaveChanges();
 
            return View("list");
        }
    }
}
