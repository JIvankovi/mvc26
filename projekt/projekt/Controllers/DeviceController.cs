using Microsoft.AspNetCore.Mvc;
using projekt.Services;
using projekt.Models;

namespace projekt.Controllers
{
    public class DeviceController : Controller
    {
        public IActionResult Index()
        {
            var devices = DataService.Devices;
            return View(devices);
        }

        public IActionResult Details(int id)
        {
            var device = DataService.Devices.FirstOrDefault(d => d.Id == id);
            if (device == null)
            {
                return NotFound();
            }
            return View(device);
        }
    }
}
