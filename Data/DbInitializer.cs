using Microsoft.AspNetCore.Identity;

namespace PlayerRoster.Server.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAdminUserAsync(IServiceProvider services)
        {
            var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

            var username = Environment.GetEnvironmentVariable("ADMIN_USER") ?? "admin";
            var password = Environment.GetEnvironmentVariable("ADMIN_PASSWORD") ?? "admin123";

            if (await userManager.FindByNameAsync(username) == null)
            {
                var user = new IdentityUser
                {
                    UserName = username,
                    Email = $"{username}@example.com",
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(user, password);
            }
        }
    }
}
