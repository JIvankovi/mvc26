using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using projekt.DTOs;
using projekt.Data;
using projekt.Models;
using projekt.ViewModels;

namespace projekt.Controllers
{
    [Route("calibrations")]
    public class CalibrationController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public CalibrationController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index(string? term = null)
        {
            var calibrations = await ApplySearch(_dbContext.Calibrations.Include(c => c.Device).Include(c => c.Technician).AsNoTracking(), term)
                .OrderByDescending(c => c.CalibrationDateTime)
                .ToListAsync();

            return View(calibrations);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(string? term)
        {
            var list = await ApplySearch(_dbContext.Calibrations.Include(c => c.Device).Include(c => c.Technician).AsNoTracking(), term)
                .OrderByDescending(c => c.CalibrationDateTime)
                .ToListAsync();
            return PartialView("_CalibrationTable", list);
        }

        [HttpGet("details/{id:int}")]
        public async Task<IActionResult> Details(int id)
        {
            var calibration = await _dbContext.Calibrations
                .Include(c => c.Device)
                .Include(c => c.Technician)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);

            if (calibration == null)
            {
                TempData["ErrorMessage"] = "Calibration record was not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(calibration);
        }

        [HttpGet("api")]
        public async Task<IActionResult> GetApi(string? term = null)
        {
            var calibrations = await ApplySearch(_dbContext.Calibrations.Include(c => c.Device).Include(c => c.Technician).AsNoTracking(), term)
                .OrderByDescending(c => c.CalibrationDateTime)
                .Select(c => new CalibrationDto
                {
                    Id = c.Id,
                    DeviceId = c.DeviceId ?? 0,
                    DeviceName = c.Device != null ? c.Device.Name : null,
                    TechnicianId = c.TechnicianId,
                    TechnicianName = c.Technician != null ? c.Technician.Name : null,
                    CalibrationDateTime = c.CalibrationDateTime,
                    CalibrationStandard = c.CalibrationStandard,
                    MeasuredDeviation = c.MeasuredDeviation,
                    PassedCalibration = c.PassedCalibration,
                    NextCalibrationDue = c.NextCalibrationDue,
                    Notes = c.Notes
                })
                .ToListAsync();

            return Ok(calibrations);
        }

        [HttpGet("api/{id:int}")]
        public async Task<IActionResult> GetApiById(int id)
        {
            var calibration = await _dbContext.Calibrations
                .Include(c => c.Device)
                .Include(c => c.Technician)
                .AsNoTracking()
                .Where(c => c.Id == id)
                .Select(c => new CalibrationDto
                {
                    Id = c.Id,
                    DeviceId = c.DeviceId ?? 0,
                    DeviceName = c.Device != null ? c.Device.Name : null,
                    TechnicianId = c.TechnicianId,
                    TechnicianName = c.Technician != null ? c.Technician.Name : null,
                    CalibrationDateTime = c.CalibrationDateTime,
                    CalibrationStandard = c.CalibrationStandard,
                    MeasuredDeviation = c.MeasuredDeviation,
                    PassedCalibration = c.PassedCalibration,
                    NextCalibrationDue = c.NextCalibrationDue,
                    Notes = c.Notes
                })
                .FirstOrDefaultAsync();

            if (calibration == null)
            {
                return NotFound();
            }

            return Ok(calibration);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("create")]
        public IActionResult Create()
        {
            return View(new CalibrationFormViewModel
            {
                CalibrationDateTime = DateTime.Now,
                PassedCalibration = true
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CalibrationFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var calibration = new Calibration
            {
                DeviceId = model.DeviceId,
                TechnicianId = model.TechnicianId,
                CalibrationDateTime = model.CalibrationDateTime,
                CalibrationStandard = model.CalibrationStandard,
                MeasuredDeviation = model.MeasuredDeviation,
                PassedCalibration = model.PassedCalibration,
                NextCalibrationDue = model.NextCalibrationDue,
                Notes = model.Notes
            };

            _dbContext.Calibrations.Add(calibration);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("api")]
        [Consumes("application/json")]
        public async Task<IActionResult> CreateApi([FromBody] CalibrationFormViewModel model)
        {
            if (!await ValidateCalibrationReferences(model))
            {
                return ValidationProblem(ModelState);
            }

            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var calibration = new Calibration
            {
                DeviceId = model.DeviceId,
                TechnicianId = model.TechnicianId,
                CalibrationDateTime = model.CalibrationDateTime,
                CalibrationStandard = model.CalibrationStandard,
                MeasuredDeviation = model.MeasuredDeviation,
                PassedCalibration = model.PassedCalibration,
                NextCalibrationDue = model.NextCalibrationDue,
                Notes = model.Notes
            };

            _dbContext.Calibrations.Add(calibration);
            await _dbContext.SaveChangesAsync();
            return Created($"/calibrations/api/{calibration.Id}", new CalibrationDto
            {
                Id = calibration.Id,
                DeviceId = calibration.DeviceId ?? 0,
                DeviceName = null,
                TechnicianId = calibration.TechnicianId,
                TechnicianName = null,
                CalibrationDateTime = calibration.CalibrationDateTime,
                CalibrationStandard = calibration.CalibrationStandard,
                MeasuredDeviation = calibration.MeasuredDeviation,
                PassedCalibration = calibration.PassedCalibration,
                NextCalibrationDue = calibration.NextCalibrationDue,
                Notes = calibration.Notes
            });
        }

        [Authorize]
        [HttpGet("edit/{id:int}")]
        public async Task<IActionResult> Edit(int id)
        {
            var calibration = await _dbContext.Calibrations
                .Include(c => c.Device)
                .Include(c => c.Technician)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (calibration == null)
            {
                ModelState.AddModelError(string.Empty, "Calibration record was not found.");
                return View(new CalibrationFormViewModel { Id = id, CalibrationDateTime = DateTime.Now });
            }

            return View(new CalibrationFormViewModel
            {
                Id = calibration.Id,
                DeviceId = calibration.DeviceId ?? 0,
                DeviceName = calibration.Device?.Name,
                TechnicianId = calibration.TechnicianId,
                TechnicianName = calibration.Technician?.Name,
                CalibrationDateTime = calibration.CalibrationDateTime,
                CalibrationStandard = calibration.CalibrationStandard,
                MeasuredDeviation = calibration.MeasuredDeviation,
                PassedCalibration = calibration.PassedCalibration,
                NextCalibrationDue = calibration.NextCalibrationDue,
                Notes = calibration.Notes
            });
        }

        [Authorize]
        [HttpPost("edit/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CalibrationFormViewModel model)
        {
            if (id != model.Id)
            {
                ModelState.AddModelError(string.Empty, "The requested calibration record does not match the submitted form.");
                return View(model);
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var calibration = await _dbContext.Calibrations.FindAsync(id);
            if (calibration == null)
            {
                ModelState.AddModelError(string.Empty, "Calibration record no longer exists.");
                return View(model);
            }

            calibration.DeviceId = model.DeviceId;
            calibration.TechnicianId = model.TechnicianId;
            calibration.CalibrationDateTime = model.CalibrationDateTime;
            calibration.CalibrationStandard = model.CalibrationStandard;
            calibration.MeasuredDeviation = model.MeasuredDeviation;
            calibration.PassedCalibration = model.PassedCalibration;
            calibration.NextCalibrationDue = model.NextCalibrationDue;
            calibration.Notes = model.Notes;

            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("api/{id:int}")]
        [Consumes("application/json")]
        public async Task<IActionResult> UpdateApi(int id, [FromBody] CalibrationFormViewModel model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }

            if (!await ValidateCalibrationReferences(model))
            {
                return ValidationProblem(ModelState);
            }

            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var calibration = await _dbContext.Calibrations.FindAsync(id);
            if (calibration == null)
            {
                return NotFound();
            }

            calibration.DeviceId = model.DeviceId;
            calibration.TechnicianId = model.TechnicianId;
            calibration.CalibrationDateTime = model.CalibrationDateTime;
            calibration.CalibrationStandard = model.CalibrationStandard;
            calibration.MeasuredDeviation = model.MeasuredDeviation;
            calibration.PassedCalibration = model.PassedCalibration;
            calibration.NextCalibrationDue = model.NextCalibrationDue;
            calibration.Notes = model.Notes;

            await _dbContext.SaveChangesAsync();
            return Ok(new CalibrationDto
            {
                Id = calibration.Id,
                DeviceId = calibration.DeviceId ?? 0,
                DeviceName = calibration.Device != null ? calibration.Device.Name : null,
                TechnicianId = calibration.TechnicianId,
                TechnicianName = calibration.Technician != null ? calibration.Technician.Name : null,
                CalibrationDateTime = calibration.CalibrationDateTime,
                CalibrationStandard = calibration.CalibrationStandard,
                MeasuredDeviation = calibration.MeasuredDeviation,
                PassedCalibration = calibration.PassedCalibration,
                NextCalibrationDue = calibration.NextCalibrationDue,
                Notes = calibration.Notes
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("delete/{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var calibration = await _dbContext.Calibrations
                .Include(c => c.Device)
                .Include(c => c.Technician)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);
            if (calibration == null)
            {
                TempData["ErrorMessage"] = "Calibration record was not found.";
                return RedirectToAction(nameof(Index));
            }

            return View(calibration);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("delete/{id:int}"), ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var calibration = await _dbContext.Calibrations.FindAsync(id);
            if (calibration == null)
            {
                TempData["ErrorMessage"] = "Calibration record no longer exists.";
                return RedirectToAction(nameof(Index));
            }

            _dbContext.Calibrations.Remove(calibration);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("api/{id:int}")]
        public async Task<IActionResult> DeleteApi(int id)
        {
            var calibration = await _dbContext.Calibrations.FindAsync(id);
            if (calibration == null)
            {
                return NotFound();
            }

            _dbContext.Calibrations.Remove(calibration);
            await _dbContext.SaveChangesAsync();
            return NoContent();
        }

        private static IQueryable<Calibration> ApplySearch(IQueryable<Calibration> query, string? term)
        {
            if (string.IsNullOrWhiteSpace(term))
            {
                return query;
            }

            term = term.Trim();
            var matchesId = int.TryParse(term, out var id);

            return query.Where(c => (matchesId && c.Id == id)
                || (c.Device != null && c.Device.Name.Contains(term))
                || (c.Technician != null && c.Technician.Name.Contains(term))
                || (c.CalibrationStandard != null && c.CalibrationStandard.Contains(term))
                || (c.Notes != null && c.Notes.Contains(term)));
        }

        private async Task<bool> ValidateCalibrationReferences(CalibrationFormViewModel model)
        {
            if (!await _dbContext.Devices.AnyAsync(d => d.Id == model.DeviceId))
            {
                ModelState.AddModelError(nameof(model.DeviceId), "Please choose a valid device.");
            }

            if (model.TechnicianId.HasValue && !await _dbContext.Technicians.AnyAsync(t => t.Id == model.TechnicianId.Value))
            {
                ModelState.AddModelError(nameof(model.TechnicianId), "Please choose a valid technician.");
            }

            return ModelState.IsValid;
        }
    }
}
