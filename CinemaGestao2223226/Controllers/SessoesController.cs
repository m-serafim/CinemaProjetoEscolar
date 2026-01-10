using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CinemaGestao2223226.Models;
using CinemaGestao2223226.Data;
using Microsoft.AspNetCore.Authorization;

namespace CinemaGestao2223226.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class SessoesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SessoesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Sessoes
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Sessoes.Include(s => s.Filme);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Sessaos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sessao = await _context.Sessoes
                .Include(s => s.Filme)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sessao == null)
            {
                return NotFound();
            }

            return View(sessao);
        }

        
        // GET: Sessoes/Create
        public IActionResult Create()
        {
            ViewData["FilmeId"] = new SelectList(_context.Filmes, "Id", "Titulo");
            return View();
        }

        // POST: Sessaos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FilmeId,DataHora,Sala,Preco,LugaresTotais,LugaresDisponiveis")] Sessao sessao)
        {
            // TEMP: to verify the POST is hit
            Console.WriteLine("DEBUG: Entered SessoesController.Create POST");

            if (ModelState.IsValid)
            {
                try
                {
                    // Set fixed room configuration (10 rows x 15 columns = 150 seats)
                    sessao.Sala = "Sala Principal";
                    sessao.LugaresTotais = 150;
                    sessao.LugaresDisponiveis = 150;
                    sessao.FilasSala = 10;
                    sessao.ColunasSala = 15;
                    
                    _context.Add(sessao);
                    await _context.SaveChangesAsync();
                    Console.WriteLine("DEBUG: Sessao saved, redirecting to Index");
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    Console.WriteLine("DEBUG: Exception while saving Sessao: " + ex);
                    throw; // let the error page show it
                }
            }

            // If we get here, ModelState is invalid
            foreach (var kvp in ModelState)
            {
                var key = kvp.Key;
                var state = kvp.Value;
                foreach (var error in state.Errors)
                {
                    Console.WriteLine($"DEBUG: ModelState error for '{key}': {error.ErrorMessage}");
                }
            }

            ViewData["FilmeId"] = new SelectList(_context.Filmes, "Id", "Titulo", sessao.FilmeId);
            return View(sessao);
        }

        // GET: Sessaos/Edit/5
        // GET: Sessoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sessao = await _context.Sessoes.FindAsync(id);
            if (sessao == null)
            {
                return NotFound();
            }
            ViewData["FilmeId"] = new SelectList(_context.Filmes, "Id", "Titulo", sessao.FilmeId);
            return View(sessao);
        }

        // POST: Sessaos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FilmeId,DataHora,Sala,Preco,LugaresTotais,LugaresDisponiveis")] Sessao sessao)
        {
            if (id != sessao.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sessao);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SessaoExists(sessao.Id))
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
            ViewData["FilmeId"] = new SelectList(_context.Filmes, "Id", "Descricao", sessao.FilmeId);
            return View(sessao);
        }

        // GET: Sessaos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sessao = await _context.Sessoes
                .Include(s => s.Filme)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sessao == null)
            {
                return NotFound();
            }

            return View(sessao);
        }

        // POST: Sessaos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sessao = await _context.Sessoes
                .Include(s => s.Filme)
                .FirstOrDefaultAsync(s => s.Id == id);
            
            if (sessao != null)
            {
                // Mark all reservations for this session as cancelled and process refunds
                var reservas = await _context.Reservas
                    .Where(r => r.SessaoId == id && !r.CanceladaPeloSistema)
                    .ToListAsync();

                foreach (var reserva in reservas)
                {
                    reserva.CanceladaPeloSistema = true;
                    reserva.MotivoCancelamento = $"A sessão do filme '{sessao.Filme?.Titulo ?? "N/A"}' em {sessao.DataHora:dd/MM/yyyy HH:mm} foi cancelada pelo administrador.";
                    reserva.Reembolsado = true;
                    reserva.DataReembolso = DateTime.Now;
                    reserva.AvisoVisualizado = false;
                    // Store snapshot of session/movie info before deletion
                    reserva.FilmeTitulo = sessao.Filme?.Titulo;
                    reserva.SessaoDataHora = sessao.DataHora;
                    _context.Update(reserva);
                }

                _context.Sessoes.Remove(sessao);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SessaoExists(int id)
        {
            return _context.Sessoes.Any(e => e.Id == id);
        }

        // POST: Sessoes/DeleteMultiple
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMultiple(int[] sessionIds)
        {
            if (sessionIds == null || sessionIds.Length == 0)
            {
                TempData["ErrorMessage"] = "Nenhuma sessão selecionada.";
                return RedirectToAction(nameof(Index));
            }

            int deletedCount = 0;
            int refundedReservations = 0;

            foreach (var sessionId in sessionIds)
            {
                var sessao = await _context.Sessoes
                    .Include(s => s.Filme)
                    .FirstOrDefaultAsync(s => s.Id == sessionId);

                if (sessao != null)
                {
                    // Mark all reservations for this session as cancelled and process refunds
                    var reservas = await _context.Reservas
                        .Where(r => r.SessaoId == sessionId && !r.CanceladaPeloSistema)
                        .ToListAsync();

                    foreach (var reserva in reservas)
                    {
                        reserva.CanceladaPeloSistema = true;
                        reserva.MotivoCancelamento = $"A sessão do filme '{sessao.Filme?.Titulo ?? "N/A"}' em {sessao.DataHora:dd/MM/yyyy HH:mm} foi cancelada pelo administrador.";
                        reserva.Reembolsado = true;
                        reserva.DataReembolso = DateTime.Now;
                        reserva.AvisoVisualizado = false;
                        // Store snapshot of session/movie info before deletion
                        reserva.FilmeTitulo = sessao.Filme?.Titulo;
                        reserva.SessaoDataHora = sessao.DataHora;
                        _context.Update(reserva);
                        refundedReservations++;
                    }

                    _context.Sessoes.Remove(sessao);
                    deletedCount++;
                }
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"{deletedCount} sessões eliminadas com sucesso! {refundedReservations} reservas foram reembolsadas.";
            return RedirectToAction(nameof(Index));
        }

        // POST: Sessoes/ResetAll - Deletes all movies, sessions, and reservations
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetAll()
        {
            // First, mark all reservations as cancelled and refunded
            var todasReservas = await _context.Reservas
                .Include(r => r.Sessao)
                .ThenInclude(s => s.Filme)
                .Where(r => !r.CanceladaPeloSistema)
                .ToListAsync();

            foreach (var reserva in todasReservas)
            {
                reserva.CanceladaPeloSistema = true;
                reserva.MotivoCancelamento = "Reset total do sistema pelo administrador. Todas as sessões e filmes foram eliminados.";
                reserva.Reembolsado = true;
                reserva.DataReembolso = DateTime.Now;
                reserva.AvisoVisualizado = false;
                // Store snapshot of session/movie info before deletion
                reserva.FilmeTitulo = reserva.Sessao?.Filme?.Titulo;
                reserva.SessaoDataHora = reserva.Sessao?.DataHora;
                _context.Update(reserva);
            }

            // Delete all sessions
            var todasSessoes = await _context.Sessoes.ToListAsync();
            _context.Sessoes.RemoveRange(todasSessoes);

            // Delete all movies
            var todosFilmes = await _context.Filmes.ToListAsync();
            _context.Filmes.RemoveRange(todosFilmes);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Reset total concluído! {todosFilmes.Count} filmes, {todasSessoes.Count} sessões eliminados. {todasReservas.Count} reservas reembolsadas.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Sessoes/AutoGenerate
        public IActionResult AutoGenerate()
        {
            ViewData["FilmeId"] = new SelectList(_context.Filmes.Where(f => f.Status == CinemaGestao2223226.Models.StatusFilme.EmCartaz), "Id", "Titulo");
            return View();
        }

        // POST: Sessoes/AutoGenerate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AutoGenerate(int filmeId, int diasSemana, string[] salas, string[] horarios, decimal preco, int lugaresTotais)
        {
            var filme = await _context.Filmes.FindAsync(filmeId);
            if (filme == null)
            {
                ModelState.AddModelError("", "Filme não encontrado.");
                ViewData["FilmeId"] = new SelectList(_context.Filmes.Where(f => f.Status == CinemaGestao2223226.Models.StatusFilme.EmCartaz), "Id", "Titulo", filmeId);
                return View();
            }

            if (salas == null || salas.Length == 0)
            {
                // Default to "Sala Principal" if no room selected
                salas = new[] { "Sala Principal" };
            }

            if (horarios == null || horarios.Length == 0)
            {
                ModelState.AddModelError("", "Selecione pelo menos um horário.");
                ViewData["FilmeId"] = new SelectList(_context.Filmes.Where(f => f.Status == CinemaGestao2223226.Models.StatusFilme.EmCartaz), "Id", "Titulo", filmeId);
                return View();
            }

            int sessoesCreated = 0;
            var startDate = DateTime.Today;

            // Fixed room configuration: 10 rows x 15 columns = 150 seats
            const int fixedLugares = 150;
            const int fixedFilas = 10;
            const int fixedColunas = 15;

            for (int day = 0; day < diasSemana; day++)
            {
                var currentDate = startDate.AddDays(day);
                
                foreach (var sala in salas)
                {
                    foreach (var horarioStr in horarios)
                    {
                        if (TimeSpan.TryParse(horarioStr, out TimeSpan horario))
                        {
                            var dataHora = currentDate.Add(horario);
                            
                            // Skip if session already exists
                            var exists = await _context.Sessoes.AnyAsync(s => 
                                s.FilmeId == filmeId && 
                                s.Sala == "Sala Principal" && 
                                s.DataHora == dataHora);
                            
                            if (!exists)
                            {
                                var sessao = new Sessao
                                {
                                    FilmeId = filmeId,
                                    DataHora = dataHora,
                                    Sala = "Sala Principal",
                                    Preco = preco,
                                    LugaresTotais = fixedLugares,
                                    LugaresDisponiveis = fixedLugares,
                                    FilasSala = fixedFilas,
                                    ColunasSala = fixedColunas
                                };
                                
                                _context.Add(sessao);
                                sessoesCreated++;
                            }
                        }
                    }
                }
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"{sessoesCreated} sessões criadas automaticamente para o filme '{filme.Titulo}'!";
            return RedirectToAction(nameof(Index));
        }
    }
}
