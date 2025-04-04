using Microsoft.AspNetCore.Http;
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
        private readonly IRepository<Incident> incidentData;
        private readonly IRepository<Customer> customerData;
        private readonly IRepository<Product> productData;
        private readonly IRepository<Technician> technicianData;

        private List<Customer> customers;
        private List<Product> products;
        private List<Technician> technicians;

        public IncidentController(
            IRepository<Incident> incidentRep,
            IRepository<Customer> customerRep,
            IRepository<Product> productRep,
            IRepository<Technician> technicianRep
            ) {

            incidentData = incidentRep;
            customerData = customerRep;
            productData = productRep;
            technicianData = technicianRep;

            customers = customerData.GetAll().OrderBy(c => c.CustomerID).ToList();
            products = productData.GetAll().OrderBy(p => p.ProductID).ToList();
            technicians = technicianData.GetAll().OrderBy(t => t.TechnicianID).ToList();
        }
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

            IQueryable<Incident> query = incidentData.GetAll().AsQueryable();


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
                Incident = incidentData.GetById(id)
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
                    incidentData.Add(incidents.Incident);
                }
                else
                {
                    incidentData.Update(incidents.Incident);
                }

                int? technicianId = HttpContext.Session.GetInt32("TechnicianID");
                
                if (technicianId.HasValue)
                {
                    var model = new TechIncidentViewModel()
                    {
                      Technicians = technicianData.GetAll().ToList(),
                        Incidents = incidentData.GetAll()
                            .Where(c => c.TechnicianID == technicianId && c.DateClosed == null)
                            .ToList(),
                        SelectedTech = technicianData.GetAll()
                            .FirstOrDefault(t => t.TechnicianID == technicianId)
                    };
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
        [HttpGet]
        public IActionResult Delete(int id)
        {
            Incident incident = incidentData.GetById(id);
            return View(incident);
        }

        [HttpPost]
        public IActionResult Delete(Incident incident)
        {
            incidentData.Delete(incident.IncidentID);
            return RedirectToAction("List");
        }

        


        public ActionResult SelectTech() {
            
            var model = new TechIncidentViewModel()
            {
                Technicians = technicianData.GetAll().ToList(),
                Incidents = incidentData.GetAll().ToList(),
                SelectedTech = new Technician()

            };
            return View("TechIncident", model);

        }
        [HttpGet]
        public ActionResult ListByTech(int Id) {
            var model = new TechIncidentViewModel()
            {
                Technicians = technicianData.GetAll().ToList(),
                Incidents = incidentData.GetAll()
                              .Where(c => c.TechnicianID == Id && c.DateClosed == null)
                              .ToList(),
                SelectedTech = new Technician()

            };
            return View("TechIncidentList", model);

        }
        [HttpPost]
        public ActionResult ListByTech(TechIncidentViewModel model) {
            int? technicianId = HttpContext.Session.GetInt32("TechnicianID");

            if (technicianId.HasValue)
            {
                HttpContext.Session.Remove("TechIncidentID");
            }


            model.SelectedTech = technicianData.GetAll()
                .FirstOrDefault(p => p.TechnicianID == model.SelectedTech.TechnicianID);

            model.Incidents = incidentData.GetAll()
                .Where(c => c.TechnicianID == model.SelectedTech.TechnicianID && c.DateClosed == null)
                .ToList();

            return View("TechIncidentList", model);

        }
    }
}
