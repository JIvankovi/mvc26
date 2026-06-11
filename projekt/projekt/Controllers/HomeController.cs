using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using projekt.Data;
using projekt.Models;

namespace projekt.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _dbContext;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task<IActionResult> Index()
        {
            var laboratories = await _dbContext.Laboratories
                .Include(l => l.DeviceLocations)
                    .ThenInclude(dl => dl.Device)
                .AsNoTracking()
                .ToListAsync();

            var technicians = await _dbContext.Technicians
                .AsNoTracking()
                .ToListAsync();

            var calibrations = await _dbContext.Calibrations
                .Include(c => c.Technician)
                .AsNoTracking()
                .ToListAsync();

            var devices = await _dbContext.Devices
                .AsNoTracking()
                .ToListAsync();

            // LINQ Query 1: Get all laboratories with their device counts grouped by building
            var laboratoryStats = laboratories
                .Select(lab => new
                {
                    Building = lab.BuildingCode,
                    LaboratoryName = lab.Name,
                    TotalDevices = lab.DeviceLocations.Count(dl => dl.IsCurrentLocation),
                    Oscilloscopes = lab.DeviceLocations
                        .Where(dl => dl.IsCurrentLocation && dl.Device != null && dl.Device.MeasurementType == MeasurementType.ElectricalSignal)
                        .Count(),
                    Voltmeters = lab.DeviceLocations
                        .Where(dl => dl.IsCurrentLocation && dl.Device != null && dl.Device.MeasurementType == MeasurementType.Voltage)
                        .Count(),
                    Barometers = lab.DeviceLocations
                        .Where(dl => dl.IsCurrentLocation && dl.Device != null && dl.Device.MeasurementType == MeasurementType.Pressure)
                        .Count(),
                    Thermometers = lab.DeviceLocations
                        .Where(dl => dl.IsCurrentLocation && dl.Device != null && dl.Device.MeasurementType == MeasurementType.Temperature)
                        .Count(),
                    Hygrometers = lab.DeviceLocations
                        .Where(dl => dl.IsCurrentLocation && dl.Device != null && dl.Device.MeasurementType == MeasurementType.Humidity)
                        .Count(),
                    Anemometers = lab.DeviceLocations
                        .Where(dl => dl.IsCurrentLocation && dl.Device != null && dl.Device.MeasurementType == MeasurementType.WindSpeed)
                        .Count(),
                    Spectrophotometers = lab.DeviceLocations
                        .Where(dl => dl.IsCurrentLocation && dl.Device != null && dl.Device.MeasurementType == MeasurementType.LightSpectrum)
                        .Count()
                })
                .ToList();

            // LINQ Query 2: Get technicians with calibration counts
            var technicianStats = technicians
                .Select(tech => new
                {
                    Technician = tech,
                    CalibrationCount = calibrations
                        .Count(c => c.Technician != null && c.Technician.Id == tech.Id),
                    SuccessRate = calibrations
                        .Where(c => c.Technician != null && c.Technician.Id == tech.Id)
                        .Any() 
                        ? (double)calibrations
                            .Where(c => c.Technician != null && c.Technician.Id == tech.Id && c.PassedCalibration)
                            .Count() / 
                          calibrations
                            .Count(c => c.Technician != null && c.Technician.Id == tech.Id) * 100
                        : 0
                })
                .OrderByDescending(x => x.CalibrationCount)
                .ToList();

            // LINQ Query 3: Get most recent calibrations
            var recentCalibrations = calibrations
                .OrderByDescending(c => c.CalibrationDateTime)
                .Take(5)
                .ToList();

            // LINQ Query 4: Devices grouped by measurement type
            var devicesByType = devices
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
