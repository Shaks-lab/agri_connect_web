using Agri_Connect.Models;
using Microsoft.EntityFrameworkCore;

namespace Agri_Connect.Data
{
    public static class ProductQueryExtensions
    {
        /// <summary>
        /// Applies the search / category / date filters to one farmer's products and
        /// returns the list plus the categories available for the filter dropdown.
        /// </summary>
        public static async Task<ProductListViewModel> ToListModelAsync(
            this IQueryable<Product> farmerProducts,
            string? q, string? category, DateTime? dateFrom, DateTime? dateTo)
        {
            var filtered = farmerProducts;

            if (!string.IsNullOrWhiteSpace(q))
            {
                var term = q.Trim();
                filtered = filtered.Where(p => p.Name.Contains(term) || p.Description.Contains(term));
            }

            if (!string.IsNullOrWhiteSpace(category))
            {
                filtered = filtered.Where(p => p.Category == category);
            }

            if (dateFrom.HasValue)
            {
                var start = dateFrom.Value.Date;
                filtered = filtered.Where(p => p.ProductionDate >= start);
            }

            if (dateTo.HasValue)
            {
                var endExclusive = dateTo.Value.Date.AddDays(1);
                filtered = filtered.Where(p => p.ProductionDate < endExclusive);
            }

            return new ProductListViewModel
            {
                Q = q,
                Category = category,
                DateFrom = dateFrom,
                DateTo = dateTo,
                Categories = await farmerProducts.Select(p => p.Category).Distinct().OrderBy(c => c).ToListAsync(),
                Products = await filtered.OrderByDescending(p => p.ProductionDate).ThenBy(p => p.Name).ToListAsync()
            };
        }
    }
}
