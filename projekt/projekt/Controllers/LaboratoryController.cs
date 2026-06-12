using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using projekt.DTOs;
using projekt.Data;
using projekt.Models;
using projekt.ViewModels;

namespace projekt.Controllers
{
    [Route("laboratories")]
    public class LaboratoryController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public LaboratoryController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index(string? term = null)
        {
            var laboratories = await ApplySearch(_dbContext.Laboratories.Include(l => l.DeviceLocations).AsNoTracking(), term)
                .OrderBy(l => l.Name)
                .ToListAsync();

            return View(laboratories);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(string? term)
        {
            var labs = await ApplySearch(_dbContext.Laboratories.Include(l => l.DeviceLocations).AsNoTracking(), term)
                .OrderBy(l => l.Name)
                .ToListAsync();
            return PartialView("_LaboratoryTable", labs);
        }

        [HttpGet("autocomplete")]
        public async Task<IActionResult> Autocomplete(string? term)
        {
            var results = await ApplySearch(_dbContext.Laboratories.AsNoTracking(), term)
                .OrderBy(l => l.Name)
                .Select(l => new { l.Id, l.Name, l.Location, l.BuildingCode })
                .Take(10)
                .ToListAsync();

            return Json(results);
        }

        [HttpGet("api")]
        public async Task<IActionResult> GetApi(string? term = null)
        {
            var labs = await ApplySearch(_dbContext.Laboratories.AsNoTracking(), term)
                .OrderBy(l => l.Name)
                .Select(l => new LaboratoryDto
                {
                    Id = l.Id,
                    Name = l.Name,
                    Location = l.Location,
                    BuildingCode = l.BuildingCode,
                    RoomNumber = l.RoomNumber,
                    ResponsiblePerson = l.ResponsiblePerson
                })
                .ToListAsync();

            return Ok(labs);
        }

        [HttpGet("api/{id:int}")]
        public async Task<IActionResult> GetApiById(int id)
        {
            var lab = await _dbContext.Laboratories
                .AsNoTracking()
                .Where(l => l.Id == id)
                .Select(l => new LaboratoryDto
                {
                    Id = l.Id,
                    Name = l.Name,
                    Location = l.Location,
                    BuildingCode = l.BuildingCode,
                    RoomNumber = l.RoomNumber,
                    ResponsiblePerson = l.ResponsiblePerson
                })
                .FirstOrDefaultAsync();

            if (lab == null)
            {
                return NotFound();
            }

            return Ok(lab);
        }

        [HttpGet("details/{id:int}")]
        public async Task<IActionResult> Details(int id)
        {
            var laboratory = await _dbContext.Laboratories
                .Include(l => l.DeviceLocations)
                    .ThenInclude(dl => dl.Device)
                .AsNoTracking()
                .FirstOrDefaultAsync(l => l.Id == id);

            if (laboratory == null)
            {
                TempData["ErrorMessage"] = "Laboratory was not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(laboratory);
        }

        [Authorize]
        [HttpGet("create")]
        public IActionResult Create() => View(new LaboratoryFormViewModel());

        [Authorize]
        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LaboratoryFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var lab = new Laboratory
            {
                Name = model.Name,
                Location = model.Location,
                BuildingCode = model.BuildingCode,
                RoomNumber = model.RoomNumber,
                ResponsiblePerson = model.ResponsiblePerson
            };

            _dbContext.Laboratories.Add(lab);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("api")]
        [Consumes("application/json")]
        public async Task<IActionResult> CreateApi([FromBody] LaboratoryFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var lab = new Laboratory
            {
                Name = model.Name,
                Location = model.Location,
                BuildingCode = model.BuildingCode,
                RoomNumber = model.RoomNumber,
                ResponsiblePerson = model.ResponsiblePerson
            };

            _dbContext.Laboratories.Add(lab);
            await _dbContext.SaveChangesAsync();
            return Created($"/laboratories/api/{lab.Id}", new LaboratoryDto
            {
                Id = lab.Id,
                Name = lab.Name,
                Location = lab.Location,
                BuildingCode = lab.BuildingCode,
                RoomNumber = lab.RoomNumber,
                ResponsiblePerson = lab.ResponsiblePerson
            });
        }

        [Authorize]
        [HttpGet("edit/{id:int}")]
        public async Task<IActionResult> Edit(int id)
        {
            var lab = await _dbContext.Laboratories.FindAsync(id);
            if (lab == null)
            {
                ModelState.AddModelError(string.Empty, "Laboratory was not found.");
                return View(new LaboratoryFormViewModel { Id = id });
            }

            return View(new LaboratoryFormViewModel
            {
                Id = lab.Id,
                Name = lab.Name,
                Location = lab.Location,
                BuildingCode = lab.BuildingCode,
                RoomNumber = lab.RoomNumber,
                ResponsiblePerson = lab.ResponsiblePerson
            });
        }

        [Authorize]
        [HttpPost("edit/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, LaboratoryFormViewModel model)
        {
            if (id != model.Id)
            {
                ModelState.AddModelError(string.Empty, "The requested laboratory does not match the submitted form.");
                return View(model);
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var lab = await _dbContext.Laboratories.FindAsync(id);
            if (lab == null)
            {
                ModelState.AddModelError(string.Empty, "Laboratory no longer exists.");
                return View(model);
            }

            lab.Name = model.Name;
            lab.Location = model.Location;
            lab.BuildingCode = model.BuildingCode;
            lab.RoomNumber = model.RoomNumber;
            lab.ResponsiblePerson = model.ResponsiblePerson;

            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("api/{id:int}")]
        [Consumes("application/json")]
        public async Task<IActionResult> UpdateApi(int id, [FromBody] LaboratoryFormViewModel model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var lab = await _dbContext.Laboratories.FindAsync(id);
            if (lab == null)
            {
                return NotFound();
            }

            lab.Name = model.Name;
            lab.Location = model.Location;
            lab.BuildingCode = model.BuildingCode;
            lab.RoomNumber = model.RoomNumber;
            lab.ResponsiblePerson = model.ResponsiblePerson;

            await _dbContext.SaveChangesAsync();
            return Ok(new LaboratoryDto
            {
                Id = lab.Id,
                Name = lab.Name,
                Location = lab.Location,
                BuildingCode = lab.BuildingCode,
                RoomNumber = lab.RoomNumber,
                ResponsiblePerson = lab.ResponsiblePerson
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("delete/{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var lab = await _dbContext.Laboratories.AsNoTracking().FirstOrDefaultAsync(l => l.Id == id);
            if (lab == null)
            {
                TempData["ErrorMessage"] = "Laboratory was not found.";
                return RedirectToAction(nameof(Index));
            }

            return View(lab);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("delete/{id:int}"), ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var lab = await _dbContext.Laboratories.FindAsync(id);
            if (lab == null)
            {
                TempData["ErrorMessage"] = "Laboratory no longer exists.";
                return RedirectToAction(nameof(Index));
            }

            _dbContext.Laboratories.Remove(lab);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("api/{id:int}")]
        public async Task<IActionResult> DeleteApi(int id)
        {
            var lab = await _dbContext.Laboratories.FindAsync(id);
            if (lab == null)
            {
                return NotFound();
            }

            _dbContext.Laboratories.Remove(lab);
            await _dbContext.SaveChangesAsync();
            return NoContent();
        }

        private static IQueryable<Laboratory> ApplySearch(IQueryable<Laboratory> query, string? term)
        {
            if (string.IsNullOrWhiteSpace(term))
            {
                return query;
            }

            term = term.Trim();
            var matchesId = int.TryParse(term, out var id);
            var matchesRoomNumber = int.TryParse(term, out var roomNumber);

            return query.Where(l => (matchesId && l.Id == id)
                || l.Name.Contains(term)
                || (l.Location != null && l.Location.Contains(term))
                || (l.BuildingCode != null && l.BuildingCode.Contains(term))
                || (matchesRoomNumber && l.RoomNumber == roomNumber)
                || (l.ResponsiblePerson != null && l.ResponsiblePerson.Contains(term)));
        }
    }
}
