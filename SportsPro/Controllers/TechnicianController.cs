using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportsPro.Models;
using System.Linq;
using System.Text.RegularExpressions;

namespace SportsPro.Controllers
{
    public class TechnicianController : Controller
    {
        private SportsProContext _context;
        public TechnicianController(SportsProContext context)
        {
            _context = context;
        }
        [HttpGet]
        [Route("/technicians")]
        public IActionResult List()
        {
            var techs = _context.Technicians.ToList();
            return View(techs);
        }


        [NonAction]

        // in NET 3.0 you can't use  _context.ChangeTracker.Clear(); 
        // so this is a solution that works with older verions
        public void ClearChangeTracker()
        {
            foreach (var entry in _context.ChangeTracker.Entries().ToList())
            {
                entry.State = EntityState.Detached;
            }
        }

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
        private void ValidateTechEditViewModel(TechEditViewModel model)
        {
            // Ensure the tech name is unique
            var tech = _context.Technicians
                .Where(t => t.TechnicianID != model.Technician!.TechnicianID && t.Name == model.Technician.Name)
                .FirstOrDefault();
      

            if (tech != null)
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


            ValidateTechEditViewModel(model);

            // Show the add form again if there were validation errors
            if (!ModelState.IsValid)
            {
                return View("Edit", model);
            }

            // Add the tech

            _context.Add(model.Technician);
            _context.SaveChanges();

            // Redirect to the tech manager page
            TempData["message"] = $"You just added the team {model.Technician.Name}.";
            return RedirectToAction("List", "Technician");
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var model = new TechEditViewModel()
            {
                Mode = "Edit",
                Technician = new Technician()
            };



            // Ensure the team exists and is owned by the user
            model.Technician = _context.Technicians
                .Where(t => t.TechnicianID == id)
                .FirstOrDefault();

            if (model.Technician == null)
            {
                return NotFound();
            }

            return View("Edit", model);
        }

        [HttpPost]
        public ActionResult Edit(int id, TechEditViewModel model)
        {


            // Ensure the tech exists and is owned by the user
            var tech = _context.Technicians
                .Where(t => t.TechnicianID == id)
                .FirstOrDefault();
            ClearChangeTracker();

            if (tech == null)
            {
                return NotFound();
            }

            // Set the appropriate tech properties and validate the tech
            model.Technician!.TechnicianID = tech.TechnicianID;
            model.Technician.Name = tech.Name;

            ValidateTechEditViewModel(model);

            // Show the edit form again if there were validation errors
            if (!ModelState.IsValid)
            {
                model.Mode = "Edit";

                return View("Edit", model);
            }

            // Update the team
            _context.Update(model.Technician);
            _context.SaveChanges();

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
            model = _context.Technicians
                .Where(c => c.TechnicianID == id)
                .FirstOrDefault();

            if (model == null)
            {
                return NotFound();
            }

            return View("Delete", model);
        }

        [HttpPost]
        public ActionResult Delete(int id, Technician model)
        {

            // Ensure the character exists and is owned by the logged in user
            var Technician = _context.Technicians
                .Where(c => c.TechnicianID == id)
                .FirstOrDefault();

            if (Technician == null)
            {
                return NotFound();
            }



            // Remove the character
            _context.Remove(Technician);
            _context.SaveChanges();

            // Return to the user's character list page
            TempData["message"] = $"You just deleted the character {Technician.Name}.";
            return RedirectToAction("List", "Technician");
        }

    }
}
