using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        public async Task<IActionResult> Index()
        {
            var calibrations = await _dbContext.Calibrations
                .Include(c => c.Device)
                .Include(c => c.Technician)
                .AsNoTracking()
                .OrderByDescending(c => c.CalibrationDateTime)
                .ToListAsync();

            return View(calibrations);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(string? term)
        {
            var query = _dbContext.Calibrations
                .Include(c => c.Device)
                .Include(c => c.Technician)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(term))
            {
                query = query.Where(c =>
                    (c.Device != null && c.Device.Name.Contains(term))
                    || (c.Technician != null && c.Technician.Name.Contains(term))
                    || (c.CalibrationStandard != null && c.CalibrationStandard.Contains(term)));
            }

            var list = await query.OrderByDescending(c => c.CalibrationDateTime).ToListAsync();
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
                return NotFound();
            }
            return View(calibration);
        }

        [HttpGet("create")]
        public IActionResult Create()
        {
            return View(new CalibrationFormViewModel
            {
                CalibrationDateTime = DateTime.Now,
                PassedCalibration = true
            });
        }

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

        [HttpGet("edit/{id:int}")]
        public async Task<IActionResult> Edit(int id)
        {
            var calibration = await _dbContext.Calibrations
                .Include(c => c.Device)
                .Include(c => c.Technician)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (calibration == null)
            {
                return NotFound();
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

        [HttpPost("edit/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CalibrationFormViewModel model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
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
            return RedirectToAction(nameof(Index));
        }

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
                return NotFound();
            }

            return View(calibration);
        }

        [HttpPost("delete/{id:int}"), ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var calibration = await _dbContext.Calibrations.FindAsync(id);
            if (calibration == null)
            {
                return NotFound();
            }

            _dbContext.Calibrations.Remove(calibration);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
