using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using SportsPro.Models;
using System.Linq;

namespace SportsPro.Controllers
{
    public class IncidentController : Controller
    {
        private SportsProContext context { get; set; }

        public IncidentController(SportsProContext ctx)
        {
            context = ctx;
        }
        public IActionResult List()
        {
            var incidents = context.Incidents.OrderBy(c => c.DateOpened).ToList();
            return View(incidents);
        }

        [HttpGet]
        [Route("/incident/add/")]
        public IActionResult Add()
        {
            // create new Incident object
            Incident incidents = new Incident();

            ViewBag.Action = "Add";

            // bind product to AddUpdate view
            return View("AddUpdate", incidents);
        }
        [HttpGet]
        [Route("/incident/{id}/edit/")]
        public IActionResult Update(int id)
        {
            Incident incidents = context.Incidents.FirstOrDefault(p => p.IncidentID == id);
            ViewBag.Action = "Edit";

            return View("AddUpdate", incidents);
        }
        [HttpPost]
        public IActionResult Update(Incident incidents)
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
                return View("AddUpdate", incidents);
            }
        }
        [HttpGet]
        [Route("/incident/{id}/delete/")]
        public IActionResult Delete(int id)
        {
            Incident incidents = context.Incidents
                .FirstOrDefault(p => p.IncidentID == id);
            return View(incidents);
        }

        [HttpPost]
        [Route("/incident/{id}/delete/")]
        public IActionResult Delete(Incident incidents)
        {
            context.Incidents.Remove(incidents);
            context.SaveChanges();
            return RedirectToAction("List");
        }
    }
}
