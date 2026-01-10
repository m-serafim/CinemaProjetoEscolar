using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CinemaGestao2223226.Models;
using CinemaGestao2223226.Data;
using Microsoft.AspNetCore.Authorization;

namespace CinemaGestao2223226.Controllers
{
    public class ReservasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReservasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Reservas - Admin sees all, Client sees only their own
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var isAdmin = User.IsInRole("Administrador");
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var reservas = isAdmin
                ? await _context.Reservas.Include(r => r.Sessao).ThenInclude(s => s.Filme).ToListAsync()
                : await _context.Reservas
                    .Include(r => r.Sessao)
                    .ThenInclude(s => s.Filme)
                    .Where(r => r.UtilizadorId == userId)
                    .ToListAsync();

            return View(reservas);
        }

        // GET: Reservas/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reserva = await _context.Reservas
                .Include(r => r.Sessao)
                .ThenInclude(s => s.Filme)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (reserva == null)
            {
                return NotFound();
            }

            // Check if user owns this reservation or is admin
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!User.IsInRole("Administrador") && reserva.UtilizadorId != userId)
            {
                return Forbid();
            }

            return View(reserva);
        }

        // GET: Reservas/Create
        [Authorize(Roles = "Cliente")]
        public IActionResult Create(int? sessaoId)
        {
            if (sessaoId == null)
            {
                ViewData["SessaoId"] = new SelectList(_context.Sessoes.Include(s => s.Filme), "Id", "Filme.Titulo");
            }
            else
            {
                ViewData["SessaoId"] = sessaoId;
                var sessao = _context.Sessoes.Include(s => s.Filme).FirstOrDefault(s => s.Id == sessaoId);
                ViewBag.Sessao = sessao;
                
                // Get occupied seats for this session
                var reservasExistentes = _context.Reservas
                    .Where(r => r.SessaoId == sessaoId && !string.IsNullOrEmpty(r.LugaresSelecionados))
                    .Select(r => r.LugaresSelecionados)
                    .ToList();
                
                var lugaresOcupados = reservasExistentes
                    .SelectMany(l => l.Split(',', StringSplitOptions.RemoveEmptyEntries))
                    .Distinct()
                    .ToList();
                
                ViewBag.LugaresOcupados = lugaresOcupados;
            }

            return View();
        }

        // POST: Reservas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Cliente")]
        public async Task<IActionResult> Create([Bind("SessaoId,NumeroBilhetes,LugaresSelecionados,NumeroCartao,NomeCartao,ValidadeCartao,CVV,ValorPago")] Reserva reserva)
        {
            var sessao = await _context.Sessoes.FindAsync(reserva.SessaoId);

            if (sessao == null)
            {
                ModelState.AddModelError("", "Sessão não encontrada.");
                return View(reserva);
            }

            if (reserva.NumeroBilhetes > sessao.LugaresDisponiveis)
            {
                ModelState.AddModelError("NumeroBilhetes", $"Apenas {sessao.LugaresDisponiveis} lugares disponíveis.");
                ViewData["SessaoId"] = reserva.SessaoId;
                ViewBag.Sessao = await _context.Sessoes.Include(s => s.Filme).FirstOrDefaultAsync(s => s.Id == reserva.SessaoId);
                return View(reserva);
            }

            // Verify selected seats are not already taken
            if (!string.IsNullOrEmpty(reserva.LugaresSelecionados))
            {
                var selectedSeats = reserva.LugaresSelecionados.Split(',', StringSplitOptions.RemoveEmptyEntries);
                var existingReservations = await _context.Reservas
                    .Where(r => r.SessaoId == reserva.SessaoId && !string.IsNullOrEmpty(r.LugaresSelecionados))
                    .Select(r => r.LugaresSelecionados)
                    .ToListAsync();
                
                var occupiedSeats = existingReservations
                    .SelectMany(l => l.Split(',', StringSplitOptions.RemoveEmptyEntries))
                    .ToHashSet();
                
                var conflictingSeats = selectedSeats.Where(s => occupiedSeats.Contains(s)).ToList();
                if (conflictingSeats.Any())
                {
                    ModelState.AddModelError("", $"Os seguintes lugares já estão ocupados: {string.Join(", ", conflictingSeats)}");
                    ViewData["SessaoId"] = reserva.SessaoId;
                    ViewBag.Sessao = await _context.Sessoes.Include(s => s.Filme).FirstOrDefaultAsync(s => s.Id == reserva.SessaoId);
                    ViewBag.LugaresOcupados = occupiedSeats.ToList();
                    return View(reserva);
                }
            }

            reserva.UtilizadorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            reserva.DataReserva = DateTime.Now;

            sessao.LugaresDisponiveis -= reserva.NumeroBilhetes;

            _context.Add(reserva);
            _context.Update(sessao);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Reserva criada com sucesso!";
            return RedirectToAction(nameof(Index));
        }

        // GET: Reservas/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reserva = await _context.Reservas
                .Include(r => r.Sessao)
                .ThenInclude(s => s.Filme)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (reserva == null)
            {
                return NotFound();
            }

            // Check if user owns this reservation or is admin
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!User.IsInRole("Administrador") && reserva.UtilizadorId != userId)
            {
                return Forbid();
            }

            return View(reserva);
        }

        // POST: Reservas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reserva = await _context.Reservas.Include(r => r.Sessao).FirstOrDefaultAsync(r => r.Id == id);
            
            if (reserva == null)
            {
                return NotFound();
            }

            // Check if user owns this reservation or is admin
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!User.IsInRole("Administrador") && reserva.UtilizadorId != userId)
            {
                return Forbid();
            }

            // Return seats to session
            reserva.Sessao.LugaresDisponiveis += reserva.NumeroBilhetes;
            _context.Update(reserva.Sessao);

            _context.Reservas.Remove(reserva);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Reservation cancelled successfully!";
            return RedirectToAction(nameof(Index));
        }

        // POST: Reservas/MarkAlertsAsViewed
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> MarkAlertsAsViewed()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var reservasCanceladas = await _context.Reservas
                .Where(r => r.UtilizadorId == userId && r.CanceladaPeloSistema && !r.AvisoVisualizado)
                .ToListAsync();

            foreach (var reserva in reservasCanceladas)
            {
                reserva.AvisoVisualizado = true;
                _context.Update(reserva);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }

        // GET: Reservas/Profits - Admin only
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Profits()
        {
            var reservas = await _context.Reservas
                .Include(r => r.Sessao)
                .ThenInclude(s => s.Filme)
                .ToListAsync();

            var sessoes = await _context.Sessoes.ToListAsync();
            var clientes = reservas.Select(r => r.UtilizadorId).Distinct().Count();

            var viewModel = new CinemaGestao2223226.ViewModels.ProfitsViewModel
            {
                TotalRevenue = reservas.Where(r => !r.Reembolsado).Sum(r => r.ValorPago),
                TotalRefunds = reservas.Where(r => r.Reembolsado).Sum(r => r.ValorPago),
                TotalTicketsSold = reservas.Where(r => !r.CanceladaPeloSistema).Sum(r => r.NumeroBilhetes),
                TotalSessions = sessoes.Count,
                TotalCustomers = clientes,
                AllReservations = reservas,
                TopMovies = reservas
                    .Where(r => r.Sessao?.Filme != null && !r.CanceladaPeloSistema)
                    .GroupBy(r => r.Sessao.Filme.Titulo)
                    .Select(g => new CinemaGestao2223226.ViewModels.MovieStats
                    {
                        MovieName = g.Key,
                        TicketsSold = g.Sum(r => r.NumeroBilhetes),
                        Revenue = g.Sum(r => r.ValorPago)
                    })
                    .OrderByDescending(m => m.TicketsSold)
                    .ToList(),
                RecentTransactions = reservas
                    .OrderByDescending(r => r.DataReserva)
                    .Take(10)
                    .Select(r => new CinemaGestao2223226.ViewModels.TransactionInfo
                    {
                        MovieName = r.Sessao?.Filme?.Titulo ?? "N/A",
                        CustomerEmail = r.UtilizadorId,
                        Amount = r.ValorPago,
                        Date = r.DataReserva,
                        IsRefund = r.Reembolsado
                    })
                    .ToList()
            };

            return View(viewModel);
        }
    }
}