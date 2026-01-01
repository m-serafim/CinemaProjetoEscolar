using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CinemaGestao.Models;
using Microsoft.AspNetCore.Authorization;
using CinemaGestao2223226.Data;
using CinemaGestao2223226.Services;
using Microsoft.AspNetCore.Hosting;

namespace CinemaGestao2223226.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class FilmesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMovieApiService _movieApiService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public FilmesController(ApplicationDbContext context, IMovieApiService movieApiService, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _movieApiService = movieApiService;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Filmes
        public async Task<IActionResult> Index()
        {
            return View(await _context.Filmes.ToListAsync());
        }

        // GET: Filmes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var filme = await _context.Filmes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (filme == null)
            {
                return NotFound();
            }

            return View(filme);
        }

        // GET: Filmes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Filmes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Titulo,Genero,DuracaoMinutos,Descricao,CapaUrl")] Filme filme)
        {
            if (ModelState.IsValid)
            {
                _context.Add(filme);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(filme);
        }

        // GET: Filmes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var filme = await _context.Filmes.FindAsync(id);
            if (filme == null)
            {
                return NotFound();
            }
            return View(filme);
        }

        // POST: Filmes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Titulo,Genero,DuracaoMinutos,Descricao,CapaUrl")] Filme filme)
        {
            if (id != filme.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(filme);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FilmeExists(filme.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(filme);
        }

        // GET: Filmes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var filme = await _context.Filmes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (filme == null)
            {
                return NotFound();
            }

            return View(filme);
        }

        // POST: Filmes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var filme = await _context.Filmes.FindAsync(id);
            if (filme != null)
            {
                _context.Filmes.Remove(filme);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FilmeExists(int id)
        {
            return _context.Filmes.Any(e => e.Id == id);
        }

        // API: Search movies from OMDb
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> SearchMovies(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return Json(new { success = false, error = "Search query cannot be empty" });
            }

            var result = await _movieApiService.SearchMoviesAsync(query);

            if (result.Response == "False")
            {
                return Json(new { success = false, error = result.Error ?? "No movies found" });
            }

            return Json(new { success = true, movies = result.Search });
        }

        // API: Get movie details from OMDb and map to Filme model
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetMovieDetails(string imdbId)
        {
            if (string.IsNullOrWhiteSpace(imdbId))
            {
                return Json(new { success = false, error = "IMDb ID cannot be empty" });
            }

            var details = await _movieApiService.GetMovieDetailsAsync(imdbId);

            if (details.Response == "False")
            {
                return Json(new { success = false, error = details.Error ?? "Movie not found" });
            }

            // Extract runtime in minutes
            int duracaoMinutos = 0;
            if (!string.IsNullOrEmpty(details.Runtime))
            {
                var match = Regex.Match(details.Runtime, @"\d+");
                if (match.Success)
                {
                    int.TryParse(match.Value, out duracaoMinutos);
                }
            }

            // Download and save poster if available
            string posterLocalPath = null;
            if (!string.IsNullOrEmpty(details.Poster) && details.Poster != "N/A")
            {
                posterLocalPath = await DownloadPoster(details.Poster, imdbId);
            }

            var filme = new
            {
                titulo = details.Title,
                genero = details.Genre,
                duracaoMinutos = duracaoMinutos,
                descricao = details.Plot,
                capaUrl = posterLocalPath ?? details.Poster
            };

            return Json(new { success = true, movie = filme });
        }

        // Helper: Download poster image and save to wwwroot/images/posters
        private async Task<string> DownloadPoster(string posterUrl, string imdbId)
        {
            try
            {
                using var httpClient = new HttpClient();
                var imageBytes = await httpClient.GetByteArrayAsync(posterUrl);

                // Ensure posters directory exists
                var postersPath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "posters");
                Directory.CreateDirectory(postersPath);

                // Generate unique filename
                var extension = Path.GetExtension(posterUrl).Split('?')[0]; // Remove query parameters
                if (string.IsNullOrEmpty(extension) || extension.Length > 5)
                {
                    extension = ".jpg";
                }
                var fileName = $"{imdbId}_{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(postersPath, fileName);

                // Save image
                await System.IO.File.WriteAllBytesAsync(filePath, imageBytes);

                // Return relative URL
                return $"/images/posters/{fileName}";
            }
            catch (Exception ex)
            {
                // Log error and return original URL
                Console.WriteLine($"Error downloading poster: {ex.Message}");
                return posterUrl;
            }
        }
    }
}
