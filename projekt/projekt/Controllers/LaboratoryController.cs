using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using projekt.Data;
using projekt.Models;
using projekt.ViewModels;

namespace projekt.Controllers
{
    [Route("laboratoriji")]
    public class LaboratoryController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public LaboratoryController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var laboratories = await _dbContext.Laboratories
                .Include(l => l.DeviceLocations)
                .AsNoTracking()
                .OrderBy(l => l.Name)
                .ToListAsync();

            return View(laboratories);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(string? term)
        {
            var query = _dbContext.Laboratories.Include(l => l.DeviceLocations).AsNoTracking();
            if (!string.IsNullOrWhiteSpace(term))
            {
                query = query.Where(l => l.Name.Contains(term)
                    || (l.Location != null && l.Location.Contains(term))
                    || (l.BuildingCode != null && l.BuildingCode.Contains(term)));
            }

            var labs = await query.OrderBy(l => l.Name).ToListAsync();
            return PartialView("_LaboratoryTable", labs);
        }

        [HttpGet("detalji/{id:int}")]
        public async Task<IActionResult> Details(int id)
        {
            var laboratory = await _dbContext.Laboratories
                .Include(l => l.DeviceLocations)
                    .ThenInclude(dl => dl.Device)
                .AsNoTracking()
                .FirstOrDefaultAsync(l => l.Id == id);

            if (laboratory == null)
            {
                return NotFound();
            }
            return View(laboratory);
        }

        [HttpGet("create")]
        public IActionResult Create() => View(new LaboratoryFormViewModel());

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

        [HttpGet("edit/{id:int}")]
        public async Task<IActionResult> Edit(int id)
        {
            var lab = await _dbContext.Laboratories.FindAsync(id);
            if (lab == null)
            {
                return NotFound();
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

        [HttpPost("edit/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, LaboratoryFormViewModel model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
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
            return RedirectToAction(nameof(Index));
        }

        [HttpGet("delete/{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var lab = await _dbContext.Laboratories.AsNoTracking().FirstOrDefaultAsync(l => l.Id == id);
            if (lab == null)
            {
                return NotFound();
            }

            return View(lab);
        }

        [HttpPost("delete/{id:int}"), ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var lab = await _dbContext.Laboratories.FindAsync(id);
            if (lab == null)
            {
                return NotFound();
            }

            _dbContext.Laboratories.Remove(lab);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
