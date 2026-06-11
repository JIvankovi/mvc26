using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using projekt.DTOs;
using projekt.Data;
using projekt.Models;
using projekt.ViewModels;

namespace projekt.Controllers
{
    [Route("technicians")]
    public class TechnicianController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public TechnicianController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index(string? term = null)
        {
            var technicians = await ApplySearch(_dbContext.Technicians.AsNoTracking(), term)
                .OrderBy(t => t.Name)
                .ToListAsync();

            return View(technicians);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(string? term)
        {
            var technicians = await ApplySearch(_dbContext.Technicians.AsNoTracking(), term)
                .OrderBy(t => t.Name)
                .ToListAsync();
            return PartialView("_TechnicianTable", technicians);
        }

        [HttpGet("autocomplete")]
        public async Task<IActionResult> Autocomplete(string? term)
        {
            var results = await ApplySearch(_dbContext.Technicians.AsNoTracking(), term)
                .OrderBy(t => t.Name)
                .Select(t => new { t.Id, t.Name, t.Email, t.Certification })
                .Take(10)
                .ToListAsync();

            return Json(results);
        }

        [HttpGet("api")]
        public async Task<IActionResult> GetApi(string? term = null)
        {
            var technicians = await ApplySearch(_dbContext.Technicians.AsNoTracking(), term)
                .OrderBy(t => t.Name)
                .Select(t => new TechnicianDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    Email = t.Email,
                    PhoneNumber = t.PhoneNumber,
                    Certification = t.Certification,
                    YearsOfExperience = t.YearsOfExperience
                })
                .ToListAsync();

            return Ok(technicians);
        }

        [HttpGet("api/{id:int}")]
        public async Task<IActionResult> GetApiById(int id)
        {
            var technician = await _dbContext.Technicians
                .AsNoTracking()
                .Where(t => t.Id == id)
                .Select(t => new TechnicianDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    Email = t.Email,
                    PhoneNumber = t.PhoneNumber,
                    Certification = t.Certification,
                    YearsOfExperience = t.YearsOfExperience
                })
                .FirstOrDefaultAsync();

            if (technician == null)
            {
                return NotFound();
            }

            return Ok(technician);
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

        [Authorize(Roles = "Admin")]
        [HttpGet("create")]
        public IActionResult Create() => View(new TechnicianFormViewModel());

        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "Admin")]
        [HttpPost("api")]
        [Consumes("application/json")]
        public async Task<IActionResult> CreateApi([FromBody] TechnicianFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
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
            return Created($"/technicians/api/{entity.Id}", new TechnicianDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Email = entity.Email,
                PhoneNumber = entity.PhoneNumber,
                Certification = entity.Certification,
                YearsOfExperience = entity.YearsOfExperience
            });
        }

        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "Admin")]
        [HttpPut("api/{id:int}")]
        [Consumes("application/json")]
        public async Task<IActionResult> UpdateApi(int id, [FromBody] TechnicianFormViewModel model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
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
            return Ok(new TechnicianDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Email = entity.Email,
                PhoneNumber = entity.PhoneNumber,
                Certification = entity.Certification,
                YearsOfExperience = entity.YearsOfExperience
            });
        }

        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "Admin")]
        [HttpDelete("api/{id:int}")]
        public async Task<IActionResult> DeleteApi(int id)
        {
            var entity = await _dbContext.Technicians.FindAsync(id);
            if (entity == null)
            {
                return NotFound();
            }

            _dbContext.Technicians.Remove(entity);
            await _dbContext.SaveChangesAsync();
            return NoContent();
        }

        private static IQueryable<Technician> ApplySearch(IQueryable<Technician> query, string? term)
        {
            if (string.IsNullOrWhiteSpace(term))
            {
                return query;
            }

            term = term.Trim();
            var matchesId = int.TryParse(term, out var id);

            return query.Where(t => (matchesId && t.Id == id)
                || t.Name.Contains(term)
                || (t.Email != null && t.Email.Contains(term))
                || (t.PhoneNumber != null && t.PhoneNumber.Contains(term))
                || (t.Certification != null && t.Certification.Contains(term)));
        }
    }
}
