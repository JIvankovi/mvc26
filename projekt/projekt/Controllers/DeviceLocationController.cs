using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        public async Task<IActionResult> Index()
        {
            var deviceLocations = await _dbContext.DeviceLocations
                .Include(dl => dl.Device)
                .Include(dl => dl.Laboratory)
                .AsNoTracking()
                .ToListAsync();

            return View(deviceLocations);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(string? term)
        {
            var query = _dbContext.DeviceLocations
                .Include(dl => dl.Device)
                .Include(dl => dl.Laboratory)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(term))
            {
                query = query.Where(dl => (dl.Device != null && dl.Device.Name.Contains(term))
                    || (dl.Laboratory != null && dl.Laboratory.Name.Contains(term))
                    || (dl.AssignmentReason != null && dl.AssignmentReason.Contains(term)));
            }

            var locations = await query.OrderBy(dl => dl.Id).ToListAsync();
            return PartialView("_DeviceLocationTable", locations);
        }

        [HttpGet("create")]
        public IActionResult Create() => View(new DeviceLocationFormViewModel());

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

        [HttpGet("edit/{id:int}")]
        public async Task<IActionResult> Edit(int id)
        {
            var entity = await _dbContext.DeviceLocations
                .Include(dl => dl.Device)
                .Include(dl => dl.Laboratory)
                .FirstOrDefaultAsync(dl => dl.Id == id);

            if (entity == null)
            {
                return NotFound();
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

        [HttpPost("edit/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DeviceLocationFormViewModel model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var entity = await _dbContext.DeviceLocations.FindAsync(id);
            if (entity == null)
            {
                return NotFound();
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
                return NotFound();
            }

            return View(entity);
        }

        [HttpPost("delete/{id:int}"), ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var entity = await _dbContext.DeviceLocations.FindAsync(id);
            if (entity == null)
            {
                return NotFound();
            }

            _dbContext.DeviceLocations.Remove(entity);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
