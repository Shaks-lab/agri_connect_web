using Agri_Connect.Data;
using Agri_Connect.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Agri_Connect.Controllers
{
    /// <summary>A farmer manages only their own products.</summary>
    [Authorize(Roles = "Farmer")]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ProductsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private async Task<Farmer?> GetCurrentFarmerAsync()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return null;
            return await _context.Farmers.AsNoTracking().FirstOrDefaultAsync(f => f.UserId == userId);
        }

        private Task<Product?> FindOwnedProductAsync(int? id, int farmerId) =>
            _context.Products.FirstOrDefaultAsync(p => p.Id == id && p.FarmerId == farmerId);

        // GET: Products?q=&category=&dateFrom=&dateTo=
        public async Task<IActionResult> Index(string? q, string? category, DateTime? dateFrom, DateTime? dateTo)
        {
            var farmer = await GetCurrentFarmerAsync();
            if (farmer == null) return View("NoProfile");

            var model = await _context.Products
                .AsNoTracking()
                .Where(p => p.FarmerId == farmer.Id)
                .ToListModelAsync(q, category, dateFrom, dateTo);
            model.Farmer = farmer;
            model.ShowActions = true;

            return View(model);
        }

        // GET: Products/Create
        public async Task<IActionResult> Create()
        {
            if (await GetCurrentFarmerAsync() == null) return View("NoProfile");
            return View(new Product { ProductionDate = DateTime.Today });
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,Price,Category,ProductionDate")] Product product)
        {
            var farmer = await GetCurrentFarmerAsync();
            if (farmer == null) return View("NoProfile");

            if (!ModelState.IsValid) return View(product);

            product.FarmerId = farmer.Id; // set by the server, never taken from the form
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Product added.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var farmer = await GetCurrentFarmerAsync();
            if (farmer == null) return View("NoProfile");

            var product = await FindOwnedProductAsync(id, farmer.Id);
            if (product == null) return NotFound();

            return View(product);
        }

        // POST: Products/Edit/5
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int? id)
        {
            var farmer = await GetCurrentFarmerAsync();
            if (farmer == null) return View("NoProfile");

            var product = await FindOwnedProductAsync(id, farmer.Id);
            if (product == null) return NotFound();

            if (await TryUpdateModelAsync(product, "",
                    p => p.Name, p => p.Description, p => p.Price, p => p.Category, p => p.ProductionDate))
            {
                await _context.SaveChangesAsync();
                TempData["Message"] = "Product updated.";
                return RedirectToAction(nameof(Index));
            }

            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var farmer = await GetCurrentFarmerAsync();
            if (farmer == null) return View("NoProfile");

            var product = await FindOwnedProductAsync(id, farmer.Id);
            if (product == null) return NotFound();

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var farmer = await GetCurrentFarmerAsync();
            if (farmer == null) return View("NoProfile");

            var product = await FindOwnedProductAsync(id, farmer.Id);
            if (product == null) return NotFound();

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Product deleted.";
            return RedirectToAction(nameof(Index));
        }
    }
}
