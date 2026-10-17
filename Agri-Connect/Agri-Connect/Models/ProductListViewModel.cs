namespace Agri_Connect.Models
{
    public class ProductListViewModel
    {
        public List<Product> Products { get; set; } = new();
        public List<string> Categories { get; set; } = new();

        public string? Q { get; set; }
        public string? Category { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }

        /// <summary>The farmer whose products are listed.</summary>
        public Farmer? Farmer { get; set; }

        /// <summary>Show Edit / Delete links (true for a farmer viewing their own products).</summary>
        public bool ShowActions { get; set; }

        public bool HasFilters =>
            !string.IsNullOrWhiteSpace(Q) || !string.IsNullOrWhiteSpace(Category) || DateFrom.HasValue || DateTo.HasValue;
    }
}
