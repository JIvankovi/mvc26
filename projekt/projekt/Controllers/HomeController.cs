using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using projekt.Models;
using projekt.Services;

namespace projekt.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            // LINQ Query 1: Get all laboratories with their device counts grouped by building
            var laboratoryStats = DataService.Laboratories
                .Select(lab => new
                {
                    Building = lab.BuildingCode,
                    LaboratoryName = lab.Name,
                    TotalDevices = lab.DeviceLocations.Count(dl => dl.IsCurrentLocation),
                    Oscilloscopes = lab.DeviceLocations
                        .Where(dl => dl.IsCurrentLocation && dl.Device is Oscilloscope)
                        .Count(),
                    Voltmeters = lab.DeviceLocations
                        .Where(dl => dl.IsCurrentLocation && dl.Device is Voltmeter)
                        .Count(),
                    Barometers = lab.DeviceLocations
                        .Where(dl => dl.IsCurrentLocation && dl.Device is Barometer)
                        .Count(),
                    Thermometers = lab.DeviceLocations
                        .Where(dl => dl.IsCurrentLocation && dl.Device is Thermometer)
                        .Count(),
                    Hygrometers = lab.DeviceLocations
                        .Where(dl => dl.IsCurrentLocation && dl.Device is Hygrometer)
                        .Count(),
                    Anemometers = lab.DeviceLocations
                        .Where(dl => dl.IsCurrentLocation && dl.Device is Anemometer)
                        .Count(),
                    Spectrophotometers = lab.DeviceLocations
                        .Where(dl => dl.IsCurrentLocation && dl.Device is Spectrophotometer)
                        .Count()
                })
                .ToList();

            // LINQ Query 2: Get technicians with calibration counts
            var technicianStats = DataService.Technicians
                .Select(tech => new
                {
                    Technician = tech,
                    CalibrationCount = DataService.Calibrations
                        .Count(c => c.Technician.Id == tech.Id),
                    SuccessRate = DataService.Calibrations
                        .Where(c => c.Technician.Id == tech.Id)
                        .Any() 
                        ? (double)DataService.Calibrations
                            .Where(c => c.Technician.Id == tech.Id && c.PassedCalibration)
                            .Count() / 
                          DataService.Calibrations
                            .Count(c => c.Technician.Id == tech.Id) * 100
                        : 0
                })
                .OrderByDescending(x => x.CalibrationCount)
                .ToList();

            // LINQ Query 3: Get most recent calibrations
            var recentCalibrations = DataService.Calibrations
                .OrderByDescending(c => c.CalibrationDateTime)
                .Take(5)
                .ToList();

            // LINQ Query 4: Devices grouped by measurement type
            var devicesByType = DataService.Devices
                .GroupBy(d => d.MeasurementType)
                .Select(g => new
                {
                    Type = g.Key,
                    Count = g.Count(),
                    Devices = g.ToList()
                })
                .OrderByDescending(x => x.Count)
                .ToList();

            // Pass all data to view using ViewBag
            ViewBag.LaboratoryStats = laboratoryStats;
            ViewBag.TechnicianStats = technicianStats;
            ViewBag.RecentCalibrations = recentCalibrations;
            ViewBag.DevicesByType = devicesByType;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
