using System.Diagnostics;
using CinemaGestao2223226.Models;
using Microsoft.AspNetCore.Mvc;
using CinemaGestao2223226.Data;
using Microsoft.EntityFrameworkCore;

namespace CinemaGestao2223226.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Fetch up to 6 recent movies to display in "Em Cartaz"
            var filmesEmCartaz = await _context.Filmes
                .Take(6)
                .ToListAsync();

            return View(filmesEmCartaz);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
