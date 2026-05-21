using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using projekt.Data;
using projekt.Models;
using projekt.ViewModels;

namespace projekt.Controllers
{
    public class TechnicianController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public TechnicianController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IActionResult> Index()
        {
            var technicians = await _dbContext.Technicians
                .AsNoTracking()
                .OrderBy(t => t.Name)
                .ToListAsync();

            return View(technicians);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(string? term)
        {
            var query = _dbContext.Technicians.AsNoTracking();
            if (!string.IsNullOrWhiteSpace(term))
            {
                query = query.Where(t => t.Name.Contains(term)
                    || (t.Email != null && t.Email.Contains(term))
                    || (t.Certification != null && t.Certification.Contains(term)));
            }

            var technicians = await query.OrderBy(t => t.Name).ToListAsync();
            return PartialView("_TechnicianTable", technicians);
        }

        public async Task<IActionResult> Details(int id)
        {
            var technician = await _dbContext.Technicians
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == id);

            if (technician == null)
            {
                return NotFound();
            }
            return View(technician);
        }

        [HttpGet("create")]
        public IActionResult Create() => View(new TechnicianFormViewModel());

        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TechnicianFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var entity = new Technician
            {
                Name = model.Name,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                Certification = model.Certification,
                YearsOfExperience = model.YearsOfExperience
            };

            _dbContext.Technicians.Add(entity);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet("edit/{id:int}")]
        public async Task<IActionResult> Edit(int id)
        {
            var entity = await _dbContext.Technicians.FindAsync(id);
            if (entity == null)
            {
                return NotFound();
            }

            return View(new TechnicianFormViewModel
            {
                Id = entity.Id,
                Name = entity.Name,
                Email = entity.Email,
                PhoneNumber = entity.PhoneNumber,
                Certification = entity.Certification,
                YearsOfExperience = entity.YearsOfExperience
            });
        }

        [HttpPost("edit/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TechnicianFormViewModel model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var entity = await _dbContext.Technicians.FindAsync(id);
            if (entity == null)
            {
                return NotFound();
            }

            entity.Name = model.Name;
            entity.Email = model.Email;
            entity.PhoneNumber = model.PhoneNumber;
            entity.Certification = model.Certification;
            entity.YearsOfExperience = model.YearsOfExperience;

            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet("delete/{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _dbContext.Technicians.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);
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
            var entity = await _dbContext.Technicians.FindAsync(id);
            if (entity == null)
            {
                return NotFound();
            }

            _dbContext.Technicians.Remove(entity);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
