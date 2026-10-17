using Microsoft.AspNetCore.Identity;

namespace Agri_Connect.Data
{
    public static class SeedData
    {
        public static readonly string[] Roles = { "Employee", "Farmer" };

        /// <summary>
        /// Makes sure the roles exist and, only if credentials are supplied through configuration
        /// (user-secrets or environment variables), creates the first Employee account.
        /// No account or password is hard-coded in the source.
        /// </summary>
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            var logger = serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("SeedData");

            foreach (var role in Roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            var email = configuration["employee@agri.com"];
            var password = configuration["Employee123!"];

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                logger.LogInformation(
                    "SeedEmployee:Email / SeedEmployee:Password are not configured, so no Employee account was created.");
                return;
            }

            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new IdentityUser { UserName = email, Email = email, EmailConfirmed = true };
                var created = await userManager.CreateAsync(user, password);
                if (!created.Succeeded)
                {
                    logger.LogError("Could not create the seed Employee account: {Errors}",
                        string.Join("; ", created.Errors.Select(e => e.Description)));
                    return;
                }
            }

            if (!await userManager.IsInRoleAsync(user, "Employee"))
            {
                await userManager.AddToRoleAsync(user, "Employee");
            }
        }
    }
}
