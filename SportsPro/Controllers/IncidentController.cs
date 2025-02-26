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
        [HttpGet]
        [Route("/incidents")]
        public IActionResult List()
        {
            var model = new IncidentListViewModel();
            model.Incidents = context.Incidents
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
                Incident = context.Incidents.FirstOrDefault(p => p.IncidentID == id)
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
                    context.Incidents.Add(incidents.Incident);
                }
                else
                {
                    context.Incidents.Update(incidents.Incident);
                }
                context.SaveChanges();
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

        [HttpGet]
        public ActionResult ListByTech() {
            var model = new TechIncidentViewModel()
            {
                Technicians = context.Technicians.ToList(),
                Incidents = context.Incidents.ToList(),
                SelectedTech = new Technician()

            };
            return View("TechIncident", model);

        }

        [HttpPost]
        public ActionResult ListByTech(TechIncidentViewModel model) {

            model.SelectedTech = context.Technicians.Where(p => p.TechnicianID == model.SelectedTech.TechnicianID).FirstOrDefault();

            model.Incidents = context.Incidents
                .Where(c => c.TechnicianID == model.SelectedTech.TechnicianID)
                .Where(c => c.DateClosed == null)
                .ToList();

            return View("TechIncidentList", model);

        }
    }
}
