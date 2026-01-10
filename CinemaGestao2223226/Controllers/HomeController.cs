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
            // Fetch all movies to display in Home page
            var todosFilmes = await _context.Filmes
                .ToListAsync();

            return View(todosFilmes);
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

            var queryLower = query.ToLower();
            var movies = await _context.Filmes
                .Where(f => f.Titulo.ToLower().Contains(queryLower))
                .Take(5)
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
            // Fetch all movies and randomize client-side for better performance
            var allMovies = await _context.Filmes
                .Select(f => new
                {
                    id = f.Id,
                    title = f.Titulo,
                    thumbnail = f.CapaUrl
                })
                .ToListAsync();

            // Randomize and take 5
            var random = new Random();
            var movies = allMovies.OrderBy(x => random.Next()).Take(5).ToList();

            return Json(new { movies = movies });
        }
    }
}
