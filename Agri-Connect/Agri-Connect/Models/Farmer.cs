using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Agri_Connect.Models
{
    public class Farmer
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Location is required")]
        public string Location { get; set; } = string.Empty;

        /// <summary>The sign-in account that belongs to this farmer.</summary>
        public string? UserId { get; set; }

        [ValidateNever]
        public IdentityUser? User { get; set; }

        [ValidateNever]
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
