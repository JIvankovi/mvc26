using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using projekt.Data;
using projekt.Models;
using projekt.ViewModels;

namespace projekt.Controllers
{
    [Route("simulation")]
    public class SimulationController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public SimulationController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var laboratories = await _dbContext.Laboratories
                .AsNoTracking()
                .Include(l => l.DeviceLocations)
                    .ThenInclude(dl => dl.Device)
                .OrderBy(l => l.Name)
                .ToListAsync();

            var model = laboratories.Select(lab => new SimulationLaboratoryViewModel
            {
                LaboratoryId = lab.Id,
                LaboratoryName = lab.Name,
                Devices = lab.DeviceLocations
                    .Where(dl => dl.IsCurrentLocation && dl.Device != null)
                    .Select(dl => new SimulationDeviceReadingViewModel
                    {
                        DeviceId = dl.Device!.Id,
                        DeviceName = dl.Device.Name,
                        ReadingValue = GenerateRandomReading(dl.Device.MeasurementType),
                        Unit = GetMeasurementUnit(dl.Device.MeasurementType),
                        MeasurementType = dl.Device.MeasurementType.ToString()
                    })
                    .OrderBy(d => d.DeviceName)
                    .ToList()
            }).ToList();

            return View(model);
        }

        private static double GenerateRandomReading(MeasurementType measurementType)
        {
            return measurementType switch
            {
                MeasurementType.Temperature => Math.Round(Random.Shared.NextDouble() * 80 - 20, 2),
                MeasurementType.Pressure => Math.Round(Random.Shared.NextDouble() * 100 + 950, 2),
                MeasurementType.Humidity => Math.Round(Random.Shared.NextDouble() * 100, 2),
                MeasurementType.WindSpeed => Math.Round(Random.Shared.NextDouble() * 40, 2),
                MeasurementType.Voltage => Math.Round(Random.Shared.NextDouble() * 240, 2),
                MeasurementType.LightSpectrum => Math.Round(Random.Shared.NextDouble() * 1000, 2),
                MeasurementType.ElectricalSignal => Math.Round(Random.Shared.NextDouble() * 5000, 2),
                _ => Math.Round(Random.Shared.NextDouble() * 100, 2)
            };
        }

        private static string GetMeasurementUnit(MeasurementType measurementType)
        {
            return measurementType switch
            {
                MeasurementType.Temperature => "degC",
                MeasurementType.Pressure => "hPa",
                MeasurementType.Humidity => "%",
                MeasurementType.WindSpeed => "m/s",
                MeasurementType.Voltage => "V",
                MeasurementType.LightSpectrum => "nm",
                MeasurementType.ElectricalSignal => "mV",
                _ => string.Empty
            };
        }
    }
}
