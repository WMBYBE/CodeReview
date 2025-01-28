using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using SportsPro.Models;
using System.Collections.Generic;
using System.Linq;

namespace SportsPro.Controllers
{
    public class IncidentController : Controller
    {
        private SportsProContext context { get; set; }
        private List<Customer> customers;
        private List<Product> products;
        private List<Technician> technicians;


        public IncidentController(SportsProContext ctx)
        {
            context = ctx;
            customers = context.Customers
                    .OrderBy(c => c.CustomerID)
                    .ToList();
            products = context.Products
                    .OrderBy(c => c.ProductID)
                    .ToList();
            technicians = context.Technicians
                    .OrderBy(c => c.TechnicianID)
                    .ToList();
        }
        public IActionResult List()
        {
            var incidents = context.Incidents
                .Include(i => i.Customer)
                .Include(i => i.Product)
                .Select(i => new IncidentViewModel
                {
                    IncidentID = i.IncidentID,
                    Title = i.Title,
                    CustomerName = i.Customer.FullName,
                    ProductName = i.Product.Name,
                    DateOpened = i.DateOpened
                }).ToList();

            return View(incidents);
        }

        [HttpGet]
        public IActionResult Add()
        {
            // create new Incident object
            Incident incidents = new Incident();

            ViewBag.Action = "Add";
            ViewBag.Customers = customers;
            ViewBag.Products = products;
            ViewBag.Technicians = technicians;

            // bind product to AddEdit view
            return View("AddEdit", incidents);
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            Incident incidents = context.Incidents.FirstOrDefault(p => p.IncidentID == id);
            ViewBag.Action = "Edit";
            ViewBag.Customers = customers;
            ViewBag.Products = products;
            ViewBag.Technicians = technicians;

            return View("AddEdit", incidents);
        }
        [HttpPost]
        public IActionResult Edit(Incident incidents)
        {
            if (ModelState.IsValid)
            {
                if (incidents.ProductID == 0)
                {
                    context.Incidents.Add(incidents);
                }
                else
                {
                    context.Incidents.Update(incidents);
                }
                context.SaveChanges();
                return RedirectToAction("List");
            }
            else
            {
                ViewBag.Action = "Save";
                ViewBag.Customers = customers;
                ViewBag.Products = products;
                ViewBag.Technicians = technicians;
                return View("AddEdit", incidents);
            }
        }
        [HttpGet]
        public IActionResult Delete(int id)
        {
            Incident incidents = context.Incidents
                .FirstOrDefault(p => p.IncidentID == id);
            return View(incidents);
        }

        [HttpPost]
        public IActionResult Delete(Incident incidents)
        {
            context.Incidents.Remove(incidents);
            context.SaveChanges();
            return RedirectToAction("List");
        }
    }
}
