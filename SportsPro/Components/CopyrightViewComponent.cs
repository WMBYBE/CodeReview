using Microsoft.AspNetCore.Mvc;
using System;

namespace SportsPro.Components
{
    public class CopyrightViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var currentYear = DateTime.Now.Year;
            return View(currentYear);
        }
    }
}
