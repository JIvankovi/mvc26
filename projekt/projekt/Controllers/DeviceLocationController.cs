using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using projekt.Data;
using projekt.Models;

namespace projekt.Controllers
{
    public class DeviceLocationController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public DeviceLocationController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IActionResult> Index()
        {
            var deviceLocations = await _dbContext.DeviceLocations
                .Include(dl => dl.Device)
                .Include(dl => dl.Laboratory)
                .AsNoTracking()
                .ToListAsync();

            return View(deviceLocations);
        }

        public async Task<IActionResult> Details(int id)
        {
            var deviceLocation = await _dbContext.DeviceLocations
                .Include(dl => dl.Device)
                .Include(dl => dl.Laboratory)
                .AsNoTracking()
                .FirstOrDefaultAsync(dl => dl.Id == id);

            if (deviceLocation == null)
            {
                return NotFound();
            }
            return View(deviceLocation);
        }
    }
}
