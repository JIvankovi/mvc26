using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using projekt.DTOs;
using projekt.Data;
using projekt.Models;
using projekt.ViewModels;

namespace projekt.Controllers
{
    [Route("devicelocations")]
    public class DeviceLocationController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public DeviceLocationController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index(string? term = null)
        {
            var deviceLocations = await ApplySearch(_dbContext.DeviceLocations.Include(dl => dl.Device).Include(dl => dl.Laboratory).AsNoTracking(), term)
                .OrderBy(dl => dl.Id)
                .ToListAsync();

            return View(deviceLocations);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(string? term)
        {
            var locations = await ApplySearch(_dbContext.DeviceLocations.Include(dl => dl.Device).Include(dl => dl.Laboratory).AsNoTracking(), term)
                .OrderBy(dl => dl.Id)
                .ToListAsync();
            return PartialView("_DeviceLocationTable", locations);
        }

        [HttpGet("details/{id:int}")]
        public async Task<IActionResult> Details(int id)
        {
            var deviceLocation = await _dbContext.DeviceLocations
                .Include(dl => dl.Device)
                .Include(dl => dl.Laboratory)
                .AsNoTracking()
                .FirstOrDefaultAsync(dl => dl.Id == id);

            if (deviceLocation == null)
            {
                TempData["ErrorMessage"] = "Device location was not found.";
                return RedirectToAction(nameof(Index));
            }

            return View(deviceLocation);
        }

        [Authorize]
        [HttpGet("create")]
        public IActionResult Create() => View(new DeviceLocationFormViewModel());

        [HttpGet("api")]
        public async Task<IActionResult> GetApi(string? term = null)
        {
            var locations = await ApplySearch(_dbContext.DeviceLocations.Include(dl => dl.Device).Include(dl => dl.Laboratory).AsNoTracking(), term)
                .OrderBy(dl => dl.Id)
                .Select(dl => new DeviceLocationDto
                {
                    Id = dl.Id,
                    DeviceId = dl.DeviceId,
                    DeviceName = dl.Device != null ? dl.Device.Name : null,
                    LaboratoryId = dl.LaboratoryId,
                    LaboratoryName = dl.Laboratory != null ? dl.Laboratory.Name : null,
                    AssignedDate = dl.AssignedDate,
                    RemovedDate = dl.RemovedDate,
                    IsCurrentLocation = dl.IsCurrentLocation,
                    AssignmentReason = dl.AssignmentReason
                })
                .ToListAsync();

            return Ok(locations);
        }

        [HttpGet("api/{id:int}")]
        public async Task<IActionResult> GetApiById(int id)
        {
            var location = await _dbContext.DeviceLocations
                .Include(dl => dl.Device)
                .Include(dl => dl.Laboratory)
                .AsNoTracking()
                .Where(dl => dl.Id == id)
                .Select(dl => new DeviceLocationDto
                {
                    Id = dl.Id,
                    DeviceId = dl.DeviceId,
                    DeviceName = dl.Device != null ? dl.Device.Name : null,
                    LaboratoryId = dl.LaboratoryId,
                    LaboratoryName = dl.Laboratory != null ? dl.Laboratory.Name : null,
                    AssignedDate = dl.AssignedDate,
                    RemovedDate = dl.RemovedDate,
                    IsCurrentLocation = dl.IsCurrentLocation,
                    AssignmentReason = dl.AssignmentReason
                })
                .FirstOrDefaultAsync();

            if (location == null)
            {
                return NotFound();
            }

            return Ok(location);
        }

        [Authorize]
        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DeviceLocationFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (!await _dbContext.Devices.AnyAsync(d => d.Id == model.DeviceId))
            {
                ModelState.AddModelError(nameof(model.DeviceId), "Please choose a valid device from the dropdown.");
                return View(model);
            }

            if (model.LaboratoryId.HasValue && !await _dbContext.Laboratories.AnyAsync(l => l.Id == model.LaboratoryId.Value))
            {
                ModelState.AddModelError(nameof(model.LaboratoryId), "Please choose a valid laboratory from the dropdown.");
                return View(model);
            }

            var entity = new DeviceLocation
            {
                DeviceId = model.DeviceId,
                LaboratoryId = model.LaboratoryId,
                AssignedDate = model.AssignedDate,
                RemovedDate = model.RemovedDate,
                IsCurrentLocation = model.IsCurrentLocation,
                AssignmentReason = model.AssignmentReason
            };

            _dbContext.DeviceLocations.Add(entity);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("api")]
        [Consumes("application/json")]
        public async Task<IActionResult> CreateApi([FromBody] DeviceLocationFormViewModel model)
        {
            if (!await ValidateDeviceLocationReferences(model))
            {
                return ValidationProblem(ModelState);
            }

            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var entity = new DeviceLocation
            {
                DeviceId = model.DeviceId,
                LaboratoryId = model.LaboratoryId,
                AssignedDate = model.AssignedDate,
                RemovedDate = model.RemovedDate,
                IsCurrentLocation = model.IsCurrentLocation,
                AssignmentReason = model.AssignmentReason
            };

            _dbContext.DeviceLocations.Add(entity);
            await _dbContext.SaveChangesAsync();
            return Created($"/devicelocations/api/{entity.Id}", new DeviceLocationDto
            {
                Id = entity.Id,
                DeviceId = entity.DeviceId,
                DeviceName = null,
                LaboratoryId = entity.LaboratoryId,
                LaboratoryName = null,
                AssignedDate = entity.AssignedDate,
                RemovedDate = entity.RemovedDate,
                IsCurrentLocation = entity.IsCurrentLocation,
                AssignmentReason = entity.AssignmentReason
            });
        }

        [Authorize]
        [HttpGet("edit/{id:int}")]
        public async Task<IActionResult> Edit(int id)
        {
            var entity = await _dbContext.DeviceLocations
                .Include(dl => dl.Device)
                .Include(dl => dl.Laboratory)
                .FirstOrDefaultAsync(dl => dl.Id == id);

            if (entity == null)
            {
                ModelState.AddModelError(string.Empty, "Device location was not found.");
                return View(new DeviceLocationFormViewModel { Id = id });
            }

            return View(new DeviceLocationFormViewModel
            {
                Id = entity.Id,
                DeviceId = entity.DeviceId,
                DeviceName = entity.Device?.Name,
                LaboratoryId = entity.LaboratoryId,
                LaboratoryName = entity.Laboratory?.Name,
                AssignedDate = entity.AssignedDate,
                RemovedDate = entity.RemovedDate,
                IsCurrentLocation = entity.IsCurrentLocation,
                AssignmentReason = entity.AssignmentReason
            });
        }

        [Authorize]
        [HttpPost("edit/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DeviceLocationFormViewModel model)
        {
            if (id != model.Id)
            {
                ModelState.AddModelError(string.Empty, "The requested device location does not match the submitted form.");
                return View(model);
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var entity = await _dbContext.DeviceLocations.FindAsync(id);
            if (entity == null)
            {
                ModelState.AddModelError(string.Empty, "Device location no longer exists.");
                return View(model);
            }

            if (!await _dbContext.Devices.AnyAsync(d => d.Id == model.DeviceId))
            {
                ModelState.AddModelError(nameof(model.DeviceId), "Please choose a valid device from the dropdown.");
                return View(model);
            }

            if (model.LaboratoryId.HasValue && !await _dbContext.Laboratories.AnyAsync(l => l.Id == model.LaboratoryId.Value))
            {
                ModelState.AddModelError(nameof(model.LaboratoryId), "Please choose a valid laboratory from the dropdown.");
                return View(model);
            }

            entity.DeviceId = model.DeviceId;
            entity.LaboratoryId = model.LaboratoryId;
            entity.AssignedDate = model.AssignedDate;
            entity.RemovedDate = model.RemovedDate;
            entity.IsCurrentLocation = model.IsCurrentLocation;
            entity.AssignmentReason = model.AssignmentReason;

            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("api/{id:int}")]
        [Consumes("application/json")]
        public async Task<IActionResult> UpdateApi(int id, [FromBody] DeviceLocationFormViewModel model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }

            if (!await ValidateDeviceLocationReferences(model))
            {
                return ValidationProblem(ModelState);
            }

            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var entity = await _dbContext.DeviceLocations.FindAsync(id);
            if (entity == null)
            {
                return NotFound();
            }

            entity.DeviceId = model.DeviceId;
            entity.LaboratoryId = model.LaboratoryId;
            entity.AssignedDate = model.AssignedDate;
            entity.RemovedDate = model.RemovedDate;
            entity.IsCurrentLocation = model.IsCurrentLocation;
            entity.AssignmentReason = model.AssignmentReason;

            await _dbContext.SaveChangesAsync();
            return Ok(new DeviceLocationDto
            {
                Id = entity.Id,
                DeviceId = entity.DeviceId,
                DeviceName = entity.Device != null ? entity.Device.Name : null,
                LaboratoryId = entity.LaboratoryId,
                LaboratoryName = entity.Laboratory != null ? entity.Laboratory.Name : null,
                AssignedDate = entity.AssignedDate,
                RemovedDate = entity.RemovedDate,
                IsCurrentLocation = entity.IsCurrentLocation,
                AssignmentReason = entity.AssignmentReason
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("delete/{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _dbContext.DeviceLocations
                .Include(dl => dl.Device)
                .Include(dl => dl.Laboratory)
                .AsNoTracking()
                .FirstOrDefaultAsync(dl => dl.Id == id);

            if (entity == null)
            {
                TempData["ErrorMessage"] = "Device location was not found.";
                return RedirectToAction(nameof(Index));
            }

            return View(entity);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("delete/{id:int}"), ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var entity = await _dbContext.DeviceLocations.FindAsync(id);
            if (entity == null)
            {
                TempData["ErrorMessage"] = "Device location no longer exists.";
                return RedirectToAction(nameof(Index));
            }

            _dbContext.DeviceLocations.Remove(entity);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("api/{id:int}")]
        public async Task<IActionResult> DeleteApi(int id)
        {
            var entity = await _dbContext.DeviceLocations.FindAsync(id);
            if (entity == null)
            {
                return NotFound();
            }

            _dbContext.DeviceLocations.Remove(entity);
            await _dbContext.SaveChangesAsync();
            return NoContent();
        }

        private static IQueryable<DeviceLocation> ApplySearch(IQueryable<DeviceLocation> query, string? term)
        {
            if (string.IsNullOrWhiteSpace(term))
            {
                return query;
            }

            term = term.Trim();
            var matchesId = int.TryParse(term, out var id);

            return query.Where(dl => (matchesId && dl.Id == id)
                || (dl.Device != null && dl.Device.Name.Contains(term))
                || (dl.Laboratory != null && dl.Laboratory.Name.Contains(term))
                || (dl.AssignmentReason != null && dl.AssignmentReason.Contains(term)));
        }

        private async Task<bool> ValidateDeviceLocationReferences(DeviceLocationFormViewModel model)
        {
            if (!await _dbContext.Devices.AnyAsync(d => d.Id == model.DeviceId))
            {
                ModelState.AddModelError(nameof(model.DeviceId), "Please choose a valid device.");
            }

            if (model.LaboratoryId.HasValue && !await _dbContext.Laboratories.AnyAsync(l => l.Id == model.LaboratoryId.Value))
            {
                ModelState.AddModelError(nameof(model.LaboratoryId), "Please choose a valid laboratory.");
            }

            return ModelState.IsValid;
        }
    }
}
