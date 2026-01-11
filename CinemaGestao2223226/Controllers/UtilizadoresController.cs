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
                var role = roles.FirstOrDefault();
                
                // Only include users that have a role (Administrador or Cliente)
                if (!string.IsNullOrEmpty(role) && (role == "Administrador" || role == "Cliente"))
                {
                    userRoleList.Add((user, role));
                }
            }

            return View(userRoleList);
        }

        // GET: Utilizadores/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Prevent admin from deleting themselves
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser.Id == id)
            {
                TempData["ErrorMessage"] = "N�o pode eliminar a sua pr�pria conta de administrador!";
                return RedirectToAction(nameof(Index));
            }

            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? "Sem Role";

            ViewBag.UserRole = role;
            return View(user);
        }

        // POST: Utilizadores/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Prevent admin from deleting themselves
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser.Id == id)
            {
                TempData["ErrorMessage"] = "N�o pode eliminar a sua pr�pria conta de administrador!";
                return RedirectToAction(nameof(Index));
            }

            var result = await _userManager.DeleteAsync(user);
            
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = $"User '{user.Email}' foi eliminado com sucesso!";
            }
            else
            {
                TempData["ErrorMessage"] = $"Error deleting user: {string.Join(", ", result.Errors.Select(e => e.Description))}";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}