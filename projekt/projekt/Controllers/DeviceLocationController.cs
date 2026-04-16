using Microsoft.AspNetCore.Mvc;
using projekt.Services;
using projekt.Models;

namespace projekt.Controllers
{
    public class DeviceLocationController : Controller
    {
        public IActionResult Index()
        {
            var deviceLocations = DataService.DeviceLocations;
            return View(deviceLocations);
        }

        public IActionResult Details(int id)
        {
            var deviceLocation = DataService.DeviceLocations.FirstOrDefault(dl => dl.Id == id);
            if (deviceLocation == null)
            {
                return NotFound();
            }
            return View(deviceLocation);
        }
    }
}
