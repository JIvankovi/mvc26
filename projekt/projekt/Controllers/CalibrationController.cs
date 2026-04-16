using Microsoft.AspNetCore.Mvc;
using projekt.Services;
using projekt.Models;

namespace projekt.Controllers
{
    public class CalibrationController : Controller
    {
        public IActionResult Index()
        {
            var calibrations = DataService.Calibrations;
            return View(calibrations);
        }

        public IActionResult Details(int id)
        {
            var calibration = DataService.Calibrations.FirstOrDefault(c => c.Id == id);
            if (calibration == null)
            {
                return NotFound();
            }
            return View(calibration);
        }
    }
}
