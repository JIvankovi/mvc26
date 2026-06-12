using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using projekt.DTOs;
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
        public async Task<IActionResult> Index(string? term = null)
        {
            var devices = await ApplySearch(_dbContext.Devices.AsNoTracking(), term)
                .OrderBy(d => d.Name)
                .ToListAsync();

            return View(devices);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(string? term)
        {
            var devices = await ApplySearch(_dbContext.Devices.AsNoTracking(), term)
                .OrderBy(d => d.Name)
                .ToListAsync();
            return PartialView("_DeviceTable", devices);
        }

        [HttpGet("autocomplete")]
        public async Task<IActionResult> Autocomplete(string? term)
        {
            var results = await ApplySearch(_dbContext.Devices.AsNoTracking(), term)
                .OrderBy(d => d.Name)
                .Select(d => new { d.Id, d.Name, d.Manufacturer, d.SerialNumber })
                .Take(10)
                .ToListAsync();

            return Json(results);
        }

        [HttpGet("api")]
        public async Task<IActionResult> GetApi(string? term = null)
        {
            var devices = await ApplySearch(_dbContext.Devices.AsNoTracking(), term)
                .OrderBy(d => d.Name)
                .Select(d => new DeviceDto
                {
                    Id = d.Id,
                    Name = d.Name,
                    Manufacturer = d.Manufacturer,
                    SerialNumber = d.SerialNumber,
                    PurchaseDate = d.PurchaseDate,
                    MeasurementType = d.MeasurementType
                })
                .ToListAsync();

            return Ok(devices);
        }

        [HttpGet("api/{id:int}")]
        public async Task<IActionResult> GetApiById(int id)
        {
            var device = await _dbContext.Devices
                .AsNoTracking()
                .Where(d => d.Id == id)
                .Select(d => new DeviceDto
                {
                    Id = d.Id,
                    Name = d.Name,
                    Manufacturer = d.Manufacturer,
                    SerialNumber = d.SerialNumber,
                    PurchaseDate = d.PurchaseDate,
                    MeasurementType = d.MeasurementType
                })
                .FirstOrDefaultAsync();

            if (device == null)
            {
                return NotFound();
            }

            return Ok(device);
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
                TempData["ErrorMessage"] = "Device was not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(device);
        }

        [Authorize]
        [HttpGet("create")]
        public IActionResult Create()
        {
            return View(new DeviceFormViewModel());
        }

        [Authorize]
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

        [Authorize(Roles = "Admin")]
        [HttpPost("api")]
        [Consumes("application/json")]
        public async Task<IActionResult> CreateApi([FromBody] DeviceFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
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
            return Created($"/devices/api/{device.Id}", new DeviceDto
            {
                Id = device.Id,
                Name = device.Name,
                Manufacturer = device.Manufacturer,
                SerialNumber = device.SerialNumber,
                PurchaseDate = device.PurchaseDate,
                MeasurementType = device.MeasurementType
            });
        }

        [Authorize]
        [HttpGet("edit/{id:int}")]
        public async Task<IActionResult> Edit(int id)
        {
            var device = await _dbContext.Devices.FindAsync(id);
            if (device == null)
            {
                ModelState.AddModelError(string.Empty, "Device was not found.");
                return View(new DeviceFormViewModel { Id = id });
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

        [Authorize]
        [HttpPost("edit/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DeviceFormViewModel model)
        {
            if (id != model.Id)
            {
                ModelState.AddModelError(string.Empty, "The requested device does not match the submitted form.");
                return View(model);
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var device = await _dbContext.Devices.FindAsync(id);
            if (device == null)
            {
                ModelState.AddModelError(string.Empty, "Device no longer exists.");
                return View(model);
            }

            device.Name = model.Name;
            device.Manufacturer = model.Manufacturer;
            device.SerialNumber = model.SerialNumber;
            device.PurchaseDate = model.PurchaseDate;
            device.MeasurementType = model.MeasurementType;

            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("api/{id:int}")]
        [Consumes("application/json")]
        public async Task<IActionResult> UpdateApi(int id, [FromBody] DeviceFormViewModel model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
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
            return Ok(new DeviceDto
            {
                Id = device.Id,
                Name = device.Name,
                Manufacturer = device.Manufacturer,
                SerialNumber = device.SerialNumber,
                PurchaseDate = device.PurchaseDate,
                MeasurementType = device.MeasurementType
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("delete/{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var device = await _dbContext.Devices
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == id);
            if (device == null)
            {
                TempData["ErrorMessage"] = "Device was not found.";
                return RedirectToAction(nameof(Index));
            }

            return View(device);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("delete/{id:int}"), ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var device = await _dbContext.Devices.FindAsync(id);
            if (device == null)
            {
                TempData["ErrorMessage"] = "Device no longer exists.";
                return RedirectToAction(nameof(Index));
            }

            _dbContext.Devices.Remove(device);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("api/{id:int}")]
        public async Task<IActionResult> DeleteApi(int id)
        {
            var device = await _dbContext.Devices.FindAsync(id);
            if (device == null)
            {
                return NotFound();
            }

            _dbContext.Devices.Remove(device);
            await _dbContext.SaveChangesAsync();
            return NoContent();
        }

        private static IQueryable<Device> ApplySearch(IQueryable<Device> query, string? term)
        {
            if (string.IsNullOrWhiteSpace(term))
            {
                return query;
            }

            term = term.Trim();
            var matchesId = int.TryParse(term, out var id);

            return query.Where(d => (matchesId && d.Id == id)
                || d.Name.Contains(term)
                || (d.Manufacturer != null && d.Manufacturer.Contains(term))
                || (d.SerialNumber != null && d.SerialNumber.Contains(term)));
        }
    }
}
