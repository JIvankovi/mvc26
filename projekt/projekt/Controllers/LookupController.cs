using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using projekt.Data;

namespace projekt.Controllers
{
    [Route("lookup")]
    public class LookupController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public LookupController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("devices")]
        public async Task<IActionResult> Devices(string? term)
        {
            var query = _dbContext.Devices.AsNoTracking();
            if (!string.IsNullOrWhiteSpace(term))
            {
                query = query.Where(d => d.Name.Contains(term) || (d.SerialNumber != null && d.SerialNumber.Contains(term)));
            }

            var results = await query
                .OrderBy(d => d.Name)
                .Take(20)
                .Select(d => new { id = d.Id, text = d.Name + (d.SerialNumber != null ? " (" + d.SerialNumber + ")" : string.Empty) })
                .ToListAsync();

            return Json(results);
        }

        [HttpGet("technicians")]
        public async Task<IActionResult> Technicians(string? term)
        {
            var query = _dbContext.Technicians.AsNoTracking();
            if (!string.IsNullOrWhiteSpace(term))
            {
                query = query.Where(t => t.Name.Contains(term) || (t.Email != null && t.Email.Contains(term)));
            }

            var results = await query
                .OrderBy(t => t.Name)
                .Take(20)
                .Select(t => new { id = t.Id, text = t.Name + (t.Email != null ? " (" + t.Email + ")" : string.Empty) })
                .ToListAsync();

            return Json(results);
        }

        [HttpGet("laboratories")]
        public async Task<IActionResult> Laboratories(string? term)
        {
            var query = _dbContext.Laboratories.AsNoTracking();
            if (!string.IsNullOrWhiteSpace(term))
            {
                query = query.Where(l => l.Name.Contains(term) || (l.Location != null && l.Location.Contains(term)));
            }

            var results = await query
                .OrderBy(l => l.Name)
                .Take(20)
                .Select(l => new { id = l.Id, text = l.Name + (l.Location != null ? " (" + l.Location + ")" : string.Empty) })
                .ToListAsync();

            return Json(results);
        }
    }
}
