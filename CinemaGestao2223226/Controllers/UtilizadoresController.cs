using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CinemaGestao2223226.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class UtilizadoresController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;

        public UtilizadoresController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        // GET: Utilizadores
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            var userRoleList = new System.Collections.Generic.List<(IdentityUser User, string Role)>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var role = roles.FirstOrDefault() ?? "Sem Role";
                userRoleList.Add((user, role));
            }

            return View(userRoleList);
        }
    }
}