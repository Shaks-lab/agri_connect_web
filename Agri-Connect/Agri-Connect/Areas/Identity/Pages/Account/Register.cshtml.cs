using Agri_Connect.Data;
using Agri_Connect.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Agri_Connect.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RegisterModel> _logger;

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ApplicationDbContext context,
            ILogger<RegisterModel> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public string? ReturnUrl { get; set; }

        public class InputModel
        {
            [Required]
            [StringLength(100)]
            [Display(Name = "Full name")]
            public string Name { get; set; } = string.Empty;

            [Required]
            [StringLength(100)]
            public string Location { get; set; } = string.Empty;

            [Required]
            [EmailAddress]
            public string Email { get; set; } = string.Empty;

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 8)]
            [DataType(DataType.Password)]
            public string Password { get; set; } = string.Empty;

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; } = string.Empty;
        }

        public void OnGet(string? returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        /// <summary>
        /// Self-registration always creates a Farmer (role + profile). The Employee role can
        /// never be chosen here; it is only given through the SeedEmployee configuration.
        /// </summary>
        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ReturnUrl = returnUrl;

            if (!ModelState.IsValid) return Page();

            var email = Input.Email.Trim();
            var user = new IdentityUser { UserName = email, Email = email };

            var result = await _userManager.CreateAsync(user, Input.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error.Description);
                return Page();
            }

            var inRole = await _userManager.AddToRoleAsync(user, "Farmer");
            if (!inRole.Succeeded)
            {
                await _userManager.DeleteAsync(user);
                foreach (var error in inRole.Errors) ModelState.AddModelError(string.Empty, error.Description);
                return Page();
            }

            try
            {
                _context.Farmers.Add(new Farmer
                {
                    Name = Input.Name.Trim(),
                    Email = email,
                    Location = Input.Location.Trim(),
                    UserId = user.Id
                });
                await _context.SaveChangesAsync();
            }
            catch
            {
                await _userManager.DeleteAsync(user);
                throw;
            }

            _logger.LogInformation("A new farmer account was registered.");

            await _signInManager.SignInAsync(user, isPersistent: false);
            return LocalRedirect(returnUrl);
        }
    }
}
