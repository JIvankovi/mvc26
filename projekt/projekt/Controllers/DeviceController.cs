using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using projekt.Data;
using projekt.Models;
using projekt.ViewModels;

namespace projekt.Controllers
{
    [Route("devices")]
    public class DeviceController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public DeviceController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var devices = await _dbContext.Devices
                .AsNoTracking()
                .OrderBy(d => d.Name)
                .ToListAsync();

            return View(devices);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(string? term)
        {
            var query = _dbContext.Devices.AsNoTracking();
            if (!string.IsNullOrWhiteSpace(term))
            {
                query = query.Where(d => d.Name.Contains(term)
                    || (d.Manufacturer != null && d.Manufacturer.Contains(term))
                    || (d.SerialNumber != null && d.SerialNumber.Contains(term)));
            }

            var devices = await query.OrderBy(d => d.Name).ToListAsync();
            return PartialView("_DeviceTable", devices);
        }

        [HttpGet("autocomplete")]
        public async Task<IActionResult> Autocomplete(string? term)
        {
            var query = _dbContext.Devices.AsNoTracking();
            if (!string.IsNullOrWhiteSpace(term))
            {
                query = query.Where(d => d.Name.Contains(term)
                    || (d.Manufacturer != null && d.Manufacturer.Contains(term))
                    || (d.SerialNumber != null && d.SerialNumber.Contains(term)));
            }

            var results = await query
                .OrderBy(d => d.Name)
                .Select(d => new { d.Id, d.Name, d.Manufacturer, d.SerialNumber })
                .Take(10)
                .ToListAsync();

            return Json(results);
        }

        [HttpGet("details/{id:int}")]
        public async Task<IActionResult> Details(int id)
        {
            var device = await _dbContext.Devices
                .Include(d => d.CalibrationHistory)
                    .ThenInclude(c => c.Technician)
                .Include(d => d.LocationHistory)
                    .ThenInclude(dl => dl.Laboratory)
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == id);

            if (device == null)
            {
                return NotFound();
            }
            return View(device);
        }

        [HttpGet("create")]
        public IActionResult Create()
        {
            return View(new DeviceFormViewModel());
        }

        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DeviceFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var device = new Device
            {
                Name = model.Name,
                Manufacturer = model.Manufacturer,
                SerialNumber = model.SerialNumber,
                PurchaseDate = model.PurchaseDate,
                MeasurementType = model.MeasurementType
            };

            _dbContext.Devices.Add(device);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet("edit/{id:int}")]
        public async Task<IActionResult> Edit(int id)
        {
            var device = await _dbContext.Devices.FindAsync(id);
            if (device == null)
            {
                return NotFound();
            }

            var vm = new DeviceFormViewModel
            {
                Id = device.Id,
                Name = device.Name,
                Manufacturer = device.Manufacturer,
                SerialNumber = device.SerialNumber,
                PurchaseDate = device.PurchaseDate,
                MeasurementType = device.MeasurementType
            };
            return View(vm);
        }

        [HttpPost("edit/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DeviceFormViewModel model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var device = await _dbContext.Devices.FindAsync(id);
            if (device == null)
            {
                return NotFound();
            }

            device.Name = model.Name;
            device.Manufacturer = model.Manufacturer;
            device.SerialNumber = model.SerialNumber;
            device.PurchaseDate = model.PurchaseDate;
            device.MeasurementType = model.MeasurementType;

            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet("delete/{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var device = await _dbContext.Devices
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == id);
            if (device == null)
            {
                return NotFound();
            }

            return View(device);
        }

        [HttpPost("delete/{id:int}"), ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var device = await _dbContext.Devices.FindAsync(id);
            if (device == null)
            {
                return NotFound();
            }

            _dbContext.Devices.Remove(device);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
