using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CinemaGestao2223226.Data;
using CinemaGestao2223226.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CinemaGestao2223226.Areas.Identity.Pages.Account.Manage
{
    public class TransactionsModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;

        public TransactionsModel(
            UserManager<IdentityUser> userManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public List<Reserva> Reservas { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            Reservas = await _context.Reservas
                .Include(r => r.Sessao)
                .ThenInclude(s => s.Filme)
                .Where(r => r.UtilizadorId == user.Id)
                .OrderByDescending(r => r.DataReserva)
                .ToListAsync();

            return Page();
        }
    }
}
