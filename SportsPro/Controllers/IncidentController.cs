using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using SportsPro.Models;
using SportsPro.Models.datalayer;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace SportsPro.Controllers
{
    public class IncidentController : Controller
    {
        private SportsProContext context { get; set; }

        private Repository<Technician> technician { get; set; }
        private Repository<Product> product { get; set; }
        private Repository<Customer> customer { get; set; }
        private Repository<Incident> incident { get; set; }
        
        private List<Customer> customers;
        private List<Product> products;
        private List<Technician> technicians;
        
        


        public IncidentController(SportsProContext ctx)
        {
            technician = new Repository<Technician>(ctx);
            product = new Repository<Product>(ctx);
            customer = new Repository<Customer>(ctx);
            incident = new Repository<Incident>(ctx);
            



            var custOptions = new QueryOptions<Customer>
            {
                OrderBy = d => d.CustomerID
            };
            var prodOptions = new QueryOptions<Product>
            {
                OrderBy = d => d.ProductID
            };
            var techOptions = new QueryOptions<Technician>
            {
                OrderBy = d => d.TechnicianID
            };

            var customers2 = customer.List(custOptions);
            var products2 = product.List(prodOptions);
            var technicians2 = technician.List(techOptions);

            technicians = technicians2.ToList();
            customers = customers2.ToList();
            products = products2.ToList();

        }

        /*
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
        [HttpGet]
        [Route("/incidents")]
        public IActionResult List()
        {
            var incinOptions = new QueryOptions<Incident>
            {
                Includes = "Product, Customer",
                
            };

            var model = new IncidentListViewModel();

            var stuff = incident.List(incinOptions);

        model.Incidents = stuff.Select(i => new IncidentViewModel
        {
            IncidentID = i.IncidentID,
            Title = i.Title,
            CustomerName = i.Customer.FullName,
            ProductName = i.Product.Name,
            DateOpened = i.DateOpened
        }).ToList(); ;
            /*model.Incidents = context.Incidents
                .Include(i => i.Customer)
                .Include(i => i.Product)
                .Select(i => new IncidentViewModel
                {
                    IncidentID = i.IncidentID,
                    Title = i.Title,
                    CustomerName = i.Customer.FullName,
                    ProductName = i.Product.Name,
                    DateOpened = i.DateOpened
                }).ToList();*/
            return View(model);
        }

        [HttpGet]
        public IActionResult Add()
        {
            // create new Incident object
            var model = new IncidentEditViewModel()
            {
                Mode = "Add",
                Technicians = technicians,
                Products = products,
                Customers = customers,
                Incident = new Incident()

            };


            // bind product to AddEdit view
            return View("AddEdit", model);
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var model = new IncidentEditViewModel()
            {
                Mode = "Edit",
                Technicians = technicians,
                Products = products,
                Customers = customers,
                Incident = incident.Get(id)

            };
            return View("AddEdit", model);
        }
        [HttpPost]
        public IActionResult Edit(IncidentEditViewModel incidents)
        {
            if (ModelState.IsValid)
            {
                if (incidents.Incident.ProductID == 0)
                {
                    incident.Insert(incidents.Incident);
                }
                else
                {
                    incident.Update(incidents.Incident);
                }
                incident.Save();
                return RedirectToAction("List");
            }
            else
            {

                incidents.Mode = "Save";
                incidents.Customers = customers;
                incidents.Products = products;
                incidents.Technicians = technicians;

                return View("AddEdit", incidents);
            }
        }
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var incident = context.Incidents.FirstOrDefault(p => p.IncidentID == id);

            var model = new DeleteConfirmationViewModel
            {
                ID = incident.IncidentID,
                Name = incident.Title
            };
            return View(model);
        }

            [HttpPost]
        public IActionResult Delete(DeleteConfirmationViewModel model)
        {
            var incident = context.Incidents.FirstOrDefault(p => p.IncidentID == model.ID);
            context.Incidents.Remove(incident);
            context.SaveChanges();
            return RedirectToAction("List");
        }

        [HttpPost]
        public IActionResult Delete(Incident incidents)
        {
            incident.Delete(incidents);
            incident.Save();
            return RedirectToAction("List");
        }
    }
}
