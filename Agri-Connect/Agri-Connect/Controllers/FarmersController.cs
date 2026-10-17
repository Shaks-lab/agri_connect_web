using Agri_Connect.Data;
using Agri_Connect.Models;
using Agri_Connect.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Agri_Connect.Controllers
{
    [Authorize(Roles = "Employee")]
    public class FarmersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public FarmersController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Farmers?q=
        public async Task<IActionResult> Index(string? q)
        {
            var query = _context.Farmers.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(q))
            {
                var term = q.Trim();
                query = query.Where(f => f.Name.Contains(term) || f.Email.Contains(term) || f.Location.Contains(term));
            }

            ViewData["Q"] = q;
            return View(await query.OrderBy(f => f.Name).ToListAsync());
        }

        // GET: Farmers/Details/5  (the farmer's products, with filters)
        public async Task<IActionResult> Details(int? id, string? q, string? category, DateTime? dateFrom, DateTime? dateTo)
        {
            if (id == null) return NotFound();

            var farmer = await _context.Farmers.AsNoTracking().FirstOrDefaultAsync(f => f.Id == id);
            if (farmer == null) return NotFound();

            var model = await _context.Products
                .AsNoTracking()
                .Where(p => p.FarmerId == farmer.Id)
                .ToListModelAsync(q, category, dateFrom, dateTo);
            model.Farmer = farmer;
            model.ShowActions = false;

            return View(model);
        }

        // GET: Farmers/Create
        public IActionResult Create() => View(new Farmer());

        // POST: Farmers/Create
        // Creates the farmer profile AND a sign-in account (role Farmer) with a one-time password.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Email,Location")] Farmer farmer)
        {
            if (!ModelState.IsValid) return View(farmer);

            farmer.Name = farmer.Name.Trim();
            farmer.Email = farmer.Email.Trim();
            farmer.Location = farmer.Location.Trim();

            if (await _userManager.FindByEmailAsync(farmer.Email) != null)
            {
                ModelState.AddModelError(nameof(Farmer.Email), "An account with this email already exists.");
                return View(farmer);
            }

            var temporaryPassword = PasswordGenerator.Generate();
            var user = new IdentityUser { UserName = farmer.Email, Email = farmer.Email, EmailConfirmed = true };

            var created = await _userManager.CreateAsync(user, temporaryPassword);
            if (!created.Succeeded)
            {
                foreach (var error in created.Errors) ModelState.AddModelError(string.Empty, error.Description);
                return View(farmer);
            }

            var inRole = await _userManager.AddToRoleAsync(user, "Farmer");
            if (!inRole.Succeeded)
            {
                await _userManager.DeleteAsync(user);
                foreach (var error in inRole.Errors) ModelState.AddModelError(string.Empty, error.Description);
                return View(farmer);
            }

            try
            {
                farmer.UserId = user.Id;
                _context.Farmers.Add(farmer);
                await _context.SaveChangesAsync();
            }
            catch
            {
                await _userManager.DeleteAsync(user);
                throw;
            }

            // Shown once on the next page, then gone.
            TempData["NewFarmerEmail"] = farmer.Email;
            TempData["NewFarmerPassword"] = temporaryPassword;
            return RedirectToAction(nameof(Index));
        }

        // GET: Farmers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var farmer = await _context.Farmers.FindAsync(id);
            if (farmer == null) return NotFound();

            return View(farmer);
        }

        // POST: Farmers/Edit/5  (name and location only; the email is the farmer's login)
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int? id)
        {
            if (id == null) return NotFound();

            var farmer = await _context.Farmers.FindAsync(id);
            if (farmer == null) return NotFound();

            if (await TryUpdateModelAsync(farmer, "", f => f.Name, f => f.Location))
            {
                farmer.Name = farmer.Name.Trim();
                farmer.Location = farmer.Location.Trim();
                await _context.SaveChangesAsync();
                TempData["Message"] = "Farmer updated.";
                return RedirectToAction(nameof(Index));
            }

            return View(farmer);
        }

        // GET: Farmers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var farmer = await _context.Farmers.AsNoTracking().FirstOrDefaultAsync(f => f.Id == id);
            if (farmer == null) return NotFound();

            ViewData["ProductCount"] = await _context.Products.CountAsync(p => p.FarmerId == farmer.Id);
            return View(farmer);
        }

        // POST: Farmers/Delete/5  (also removes the farmer's products and sign-in account)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var farmer = await _context.Farmers.FindAsync(id);
            if (farmer == null) return RedirectToAction(nameof(Index));

            var userId = farmer.UserId;

            _context.Farmers.Remove(farmer); // products are removed by the cascade
            await _context.SaveChangesAsync();

            if (userId != null)
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null && !await _userManager.IsInRoleAsync(user, "Employee"))
                {
                    await _userManager.DeleteAsync(user);
                }
            }

            TempData["Message"] = "Farmer deleted.";
            return RedirectToAction(nameof(Index));
        }
    }
}
