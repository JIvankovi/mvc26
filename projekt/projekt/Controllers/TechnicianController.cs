using Microsoft.AspNetCore.Mvc;
using projekt.Services;
using projekt.Models;

namespace projekt.Controllers
{
    public class TechnicianController : Controller
    {
        public IActionResult Index()
        {
            var technicians = DataService.Technicians;
            return View(technicians);
        }

        public IActionResult Details(int id)
        {
            var technician = DataService.Technicians.FirstOrDefault(t => t.Id == id);
            if (technician == null)
            {
                return NotFound();
            }
            return View(technician);
        }
    }
}
