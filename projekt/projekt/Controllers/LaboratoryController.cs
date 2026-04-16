using Microsoft.AspNetCore.Mvc;
using projekt.Services;
using projekt.Models;

namespace projekt.Controllers
{
    public class LaboratoryController : Controller
    {
        public IActionResult Index()
        {
            var laboratories = DataService.Laboratories;
            return View(laboratories);
        }

        public IActionResult Details(int id)
        {
            var laboratory = DataService.Laboratories.FirstOrDefault(l => l.Id == id);
            if (laboratory == null)
            {
                return NotFound();
            }
            return View(laboratory);
        }
    }
}
