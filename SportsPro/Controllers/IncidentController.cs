using Microsoft.AspNetCore.Http;
﻿using Microsoft.AspNetCore.Authorization;
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
        
        [Authorize(Roles = "Admin,User")]

        [HttpGet]
        [Route("/incidents/{filter?}")]
        public IActionResult List(string filter = "all")
        {
            int? technicianId = HttpContext.Session.GetInt32("TechnicianID");

            if (technicianId.HasValue)
            {
                HttpContext.Session.Remove("TechIncidentID");
            }
            var model = new IncidentListViewModel();

            IQueryable<Incident> query = context.Incidents
                .Include(i => i.Customer)
                .Include(i => i.Product);

            switch (filter.ToLower())
            {
                case "unassigned":
                    query = query.Where(i => i.TechnicianID == null);
                    break;
                case "open":
                    query = query.Where(i => i.DateClosed == null);
                    break;
                case "all":
                default:
                    break;
            }

            model.Incidents = query
                .Select(i => new IncidentViewModel
                {
                    IncidentID = i.IncidentID,
                    Title = i.Title,
                    CustomerName = i.Customer.FullName,
                    ProductName = i.Product.Name,
                    DateOpened = i.DateOpened
                }).ToList();

            model.Filter = filter;

            return View(model);
        }


        [Authorize(Roles = "Admin,User")]

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

        [Authorize(Roles = "Admin,User")]

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

        [Authorize(Roles = "Admin,User")]

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

                int? technicianId = HttpContext.Session.GetInt32("TechnicianID");
                
                if (technicianId.HasValue)
                {
                    var model = new TechIncidentViewModel()
                    {
                        Technicians = context.Technicians.ToList(),
                        Incidents = context.Incidents.ToList(),
                        SelectedTech = new Technician()

                    };

                    model.SelectedTech = context.Technicians.Where(p => p.TechnicianID == technicianId).FirstOrDefault();

                    model.Incidents = context.Incidents
                        .Where(c => c.TechnicianID == technicianId)
                        .Where(c => c.DateClosed == null)
                        .ToList();
                    return View("TechIncidentList", model);
                } else
                {
                return RedirectToAction("List");

                }
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
        [Authorize(Roles = "Admin")]

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
        [Authorize(Roles = "Admin")]

            [HttpPost]
        public IActionResult Delete(DeleteConfirmationViewModel model)
        {
            var incident = context.Incidents.FirstOrDefault(p => p.IncidentID == model.ID);
            context.Incidents.Remove(incident);
            context.SaveChanges();
            return RedirectToAction("List");
        }

        


        public ActionResult SelectTech() {
            
            var model = new TechIncidentViewModel()
            {
                Technicians = context.Technicians.ToList(),
                Incidents = context.Incidents.ToList(),
                SelectedTech = new Technician()

            };
            return View("TechIncident", model);

        }
        [HttpGet]
        public ActionResult ListByTech(int Id) {
            var model = new TechIncidentViewModel()
            {
                Technicians = context.Technicians.ToList(),
                Incidents = context.Incidents.ToList(),
                SelectedTech = new Technician()

            };

            model.SelectedTech = context.Technicians.Where(p => p.TechnicianID == Id).FirstOrDefault();

            model.Incidents = context.Incidents
                .Where(c => c.TechnicianID == Id)
                .Where(c => c.DateClosed == null)
                .ToList();

            return View("TechIncidentList", model);

        }
        [HttpPost]
        public ActionResult ListByTech(TechIncidentViewModel model) {
            int? technicianId = HttpContext.Session.GetInt32("TechnicianID");

            if (technicianId.HasValue)
            {
                HttpContext.Session.Remove("TechIncidentID");
            }

            model.SelectedTech = context.Technicians.Where(p => p.TechnicianID == model.SelectedTech.TechnicianID).FirstOrDefault();

            model.Incidents = context.Incidents
                .Where(c => c.TechnicianID == model.SelectedTech.TechnicianID)
                .Where(c => c.DateClosed == null)
                .ToList();

            return View("TechIncidentList", model);

        }
    }
}
