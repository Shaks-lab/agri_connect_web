using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Agri_Connect.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter a product name")]
        [StringLength(100, MinimumLength = 3)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter a product description")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter a price")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Please enter a positive price")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Please specify a category")]
        public string Category { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        [Display(Name = "Production Date")]
        [Required(ErrorMessage = "Please enter the production date")]
        public DateTime ProductionDate { get; set; }

        /// <summary>The farmer who owns this product. Always set by the server, never by the form.</summary>
        public int FarmerId { get; set; }

        [ValidateNever]
        public Farmer? Farmer { get; set; }
    }
}
