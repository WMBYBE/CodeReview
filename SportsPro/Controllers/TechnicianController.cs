using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportsPro.Models;
using SportsPro.Models.datalayer;
using System.Linq;
using System.Text.RegularExpressions;

namespace SportsPro.Controllers
{
    public class TechnicianController : Controller
    {
<<<<<<< HEAD
        private IRepository<Technician> technicianData { get; set; }

        public TechnicianController(IRepository<Technician> rep)
        {
            technicianData = rep;
=======
        private Repository<Technician> tech { get; set; }

        public TechnicianController(SportsProContext ctx)
        { 
            tech = new Repository<Technician>(ctx);
>>>>>>> Blade-Branch
        }

        [HttpGet]
        [Route("/technicians")]
        public IActionResult List()
        {
<<<<<<< HEAD
            var techs = technicianData.GetAll().ToList();
            return View(techs);
        }

=======
            var techOptions = new QueryOptions<Technician>
            {
                OrderBy = d => d.TechnicianID
            };

            var techs = tech.List(techOptions);
            return View(techs);
        }


        /*      What does this do??? Is it necessary and part of the app?
         * 
         * [NonAction]

        // in NET 3.0 you can't use  _context.ChangeTracker.Clear(); 
        // so this is a solution that works with older verions
        public void ClearChangeTracker()
        {
            foreach (var entry in _context.ChangeTracker.Entries().ToList())
            {
                entry.State = EntityState.Detached;
            }
        }*/

>>>>>>> Blade-Branch
        [NonAction]
        public bool IsValidEmail(string email)
        {
            var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            return regex.IsMatch(email);
        }

        [NonAction]
        public bool IsValidPhone(string phone)
        {
            var regex = new Regex(@"^\d{3}-\d{3}-\d{4}$");
            return regex.IsMatch(phone);
        }

        [NonAction]
        private void ValidateTechEditViewModel(TechEditViewModel model, IRepository<Technician> techData)
        {
            // Ensure the tech name is unique
<<<<<<< HEAD
            var tech = techData.GetAll()
=======
            /*
            var techs = tech
>>>>>>> Blade-Branch
                .Where(t => t.TechnicianID != model.Technician!.TechnicianID && t.Name == model.Technician.Name)
                .FirstOrDefault();
            */

            var techOptions = new QueryOptions<Technician>
            {
                Where = t => t.TechnicianID != model.Technician!.TechnicianID && t.Name == model.Technician.Name
            };

            var techs2 = tech.List(techOptions);

            if (techs2 != null)
            {
                ModelState.AddModelError("Character.Name", "You already have a character with that name.");
            }
            if (!IsValidEmail(model.Technician.Email))
            {
                ModelState.AddModelError("Email", "Invalid email format.");
            }
            if (!IsValidPhone(model.Technician.Phone))
            {
                ModelState.AddModelError("Phone Number", "Invalid Phone Number format.");
            }

        }


        [HttpGet]
        public ActionResult Add()
        {
            var model = new TechEditViewModel()
            {
                Mode = "Add",
                Technician = new Technician()
            };


            return View("Edit", model);
        }

        [HttpPost]
        public ActionResult Add(TechEditViewModel model)
        {
            model.Mode = "Add";

            // Ensure the user is logged in


            ValidateTechEditViewModel(model, technicianData);

            // Show the add form again if there were validation errors
            if (!ModelState.IsValid)
            {
                return View("Edit", model);
            }

            // Add the tech

<<<<<<< HEAD
            technicianData.Add(model.Technician);

=======
            tech.Insert(model.Technician);
            tech.Save();
>>>>>>> Blade-Branch

            // Redirect to the tech manager page
            TempData["message"] = $"You just added the team {model.Technician.Name}.";
            return RedirectToAction("List", "Technician");
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {            
            var technician = technicianData.GetAll()
               .FirstOrDefault(t => t.TechnicianID == id);


            if (technician == null)
            {
                return NotFound();
            }

            var model = new TechEditViewModel()
            {
                Mode = "Edit",
                Technician = new Technician()
            };



<<<<<<< HEAD
=======
            // Ensure the team exists and is owned by the user
            /*
            model.Technician = _context.Technicians
                .Where(t => t.TechnicianID == id)
                .FirstOrDefault();
            */

            model.Technician = tech.Get(id);

            if (model.Technician == null)
            {
                return NotFound();
            }
>>>>>>> Blade-Branch

            return View("Edit", model);
        }

        [HttpPost]
        public ActionResult Edit(int id, TechEditViewModel model)
        {


            // Ensure the tech exists and is owned by the user
<<<<<<< HEAD
            var tech = technicianData.GetAll()
                         .FirstOrDefault(t => t.TechnicianID == id);
=======
            /*
            var techs = _context.Technicians
                .Where(t => t.TechnicianID == id)
                .FirstOrDefault();
            */
>>>>>>> Blade-Branch

            var techs2 = tech.Get(id);

            //ClearChangeTracker();

            if (techs2 == null)
            {
                return NotFound();
            }

            // Set the appropriate tech properties and validate the tech
            model.Technician!.TechnicianID = techs2.TechnicianID;
            model.Technician.Name = techs2.Name;

            ValidateTechEditViewModel(model, technicianData);

            // Show the edit form again if there were validation errors
            if (!ModelState.IsValid)
            {
                model.Mode = "Edit";

                return View("Edit", model);
            }

            // Update the team
<<<<<<< HEAD
            technicianData.Update(model.Technician);

=======
            tech.Update(model.Technician);
            tech.Save();
>>>>>>> Blade-Branch

            // Redirect to the user's teams page
            TempData["message"] = $"You just edited the team {model.Technician.Name}.";
            return RedirectToAction("List", "Technician");
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            // Ensure the user is logged in
            var model = new Technician();


            // Verify the character exists and is owned by the logged in user
<<<<<<< HEAD
            model = technicianData.GetAll()
                .FirstOrDefault(t => t.TechnicianID == id);
=======
            /*
            model = _context.Technicians
                .Where(c => c.TechnicianID == id)
                .FirstOrDefault();
            */
            var model2 = tech.Get(id);
>>>>>>> Blade-Branch

            if (model2 == null)
            {
                return NotFound();
            }

            return View("Delete", model2);
        }

        [HttpPost]
        public ActionResult Delete(int id, Technician model)
        {

            // Ensure the character exists and is owned by the logged in user
<<<<<<< HEAD
            var Technician = technicianData.GetAll()
                .FirstOrDefault(t => t.TechnicianID == id);
=======
            /*
            var Technician = _context.Technicians
                .Where(c => c.TechnicianID == id)
                .FirstOrDefault();
            */
            var Technician2 = tech.Get(id);
>>>>>>> Blade-Branch

            if (Technician2 == null)
            {
                return NotFound();
            }

            // Remove the character
<<<<<<< HEAD
            technicianData.Delete(id);
            
=======
            tech.Delete(Technician2);
            tech.Save();

>>>>>>> Blade-Branch
            // Return to the user's character list page
            TempData["message"] = $"You just deleted the character {Technician2.Name}.";
            return RedirectToAction("List", "Technician");
        }

        
    }
}
