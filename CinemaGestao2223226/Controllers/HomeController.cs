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

        // API endpoint for live movie search
        [HttpGet]
        public async Task<IActionResult> SearchMovies(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return Json(new { results = new List<object>() });
            }

            var movies = await _context.Filmes
                .Where(f => f.Titulo.Contains(query))
                .Take(3)
                .Select(f => new
                {
                    id = f.Id,
                    title = f.Titulo,
                    genre = f.Genero,
                    thumbnail = f.CapaUrl
                })
                .ToListAsync();

            return Json(new { results = movies });
        }

        // API endpoint for rotating movie display
        [HttpGet]
        public async Task<IActionResult> GetRandomMovies()
        {
            var movies = await _context.Filmes
                .OrderBy(f => Guid.NewGuid())
                .Take(5)
                .Select(f => new
                {
                    id = f.Id,
                    title = f.Titulo,
                    thumbnail = f.CapaUrl
                })
                .ToListAsync();

            return Json(new { movies = movies });
        }
    }
}
