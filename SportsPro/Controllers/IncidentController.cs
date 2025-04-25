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
<<<<<<< HEAD
        private readonly IRepository<Incident> incidentData;
        private readonly IRepository<Customer> customerData;
        private readonly IRepository<Product> productData;
        private readonly IRepository<Technician> technicianData;
        private ISession session { get; set; }
        private IRequestCookieCollection requestCookies { get; set; } 
        private IResponseCookies responseCookies { get; set; }

=======
        private SportsProContext context { get; set; }

        private Repository<Technician> technician { get; set; }
        private Repository<Product> product { get; set; }
        private Repository<Customer> customer { get; set; }
        private Repository<Incident> incident { get; set; }
        
>>>>>>> Blade-Branch
        private List<Customer> customers;
        private List<Product> products;
        private List<Technician> technicians;
        
        

        public IncidentController(
            IRepository<Incident> incidentRep,
            IRepository<Customer> customerRep,
            IRepository<Product> productRep,
            IRepository<Technician> technicianRep,
            IHttpContextAccessor ctx
            ) {

<<<<<<< HEAD
            incidentData = incidentRep;
            customerData = customerRep;
            productData = productRep;
            technicianData = technicianRep;
            session = ctx.HttpContext!.Session;
            requestCookies = ctx.HttpContext!.Request.Cookies; 
            responseCookies = ctx.HttpContext!.Response.Cookies;

            customers = customerData.GetAll().OrderBy(c => c.CustomerID).ToList();
            products = productData.GetAll().OrderBy(p => p.ProductID).ToList();
            technicians = technicianData.GetAll().OrderBy(t => t.TechnicianID).ToList();
=======
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
>>>>>>> Blade-Branch
        }
        */

        [HttpGet]
        [Route("/incidents/{filter?}")]
        public IActionResult List(string filter = "all")
        {
<<<<<<< HEAD
            int? technicianId = session.GetInt32("TechnicianID");

            if (technicianId.HasValue)
            {
                session.Remove("TechIncidentID");
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
=======
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
>>>>>>> Blade-Branch
                .Select(i => new IncidentViewModel
                {
                    IncidentID = i.IncidentID,
                    Title = i.Title,
                    CustomerName = i.Customer.FullName,
                    ProductName = i.Product.Name,
                    DateOpened = i.DateOpened
<<<<<<< HEAD
                }).ToList();

            model.Filter = filter;

=======
                }).ToList();*/
>>>>>>> Blade-Branch
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
<<<<<<< HEAD
                Incident = incidentData.GetById(id)
=======
                Incident = incident.Get(id)

>>>>>>> Blade-Branch
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
<<<<<<< HEAD
                    incidentData.Add(incidents.Incident);
                }
                else
                {
                    incidentData.Update(incidents.Incident);
                }

                int? technicianId = session.GetInt32("TechnicianID");
                
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
=======
                    incident.Insert(incidents.Incident);
                }
                else
                {
                    incident.Update(incidents.Incident);
                }
                incident.Save();
>>>>>>> Blade-Branch
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
<<<<<<< HEAD
            Incident incident = incidentData.GetById(id);
            return View(incident);
=======
            Incident incidents = incident.Get(id);
            return View(incidents);
>>>>>>> Blade-Branch
        }

        [HttpPost]
        public IActionResult Delete(Incident incident)
        {
<<<<<<< HEAD
            incidentData.Delete(incident.IncidentID);
=======
            incident.Delete(incidents);
            incident.Save();
>>>>>>> Blade-Branch
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
            int? technicianId = session.GetInt32("TechnicianID");

            if (technicianId.HasValue)
            {
                session.Remove("TechIncidentID");
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
