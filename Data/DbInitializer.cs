using Microsoft.AspNetCore.Identity;

public static class DbInitializer
{
    public static async Task SeedAdminUserAsync(IServiceProvider serviceProvider)
    {
        try
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            string adminUser = Environment.GetEnvironmentVariable("ADMIN_USER") ?? "admin";
            string adminPass = Environment.GetEnvironmentVariable("ADMIN_PASSWORD") ?? "admin123";

            if (!await roleManager.RoleExistsAsync("Admin"))
                await roleManager.CreateAsync(new IdentityRole("Admin"));

            var user = await userManager.FindByNameAsync(adminUser);
            if (user == null)
            {
                user = new IdentityUser
                {
                    UserName = adminUser,
                    Email = $"{adminUser}@example.com",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, adminPass);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Admin");
                    Console.WriteLine("✅ Admin user seeded.");
                }
                else
                {
                    Console.WriteLine("❌ Failed to seed admin user: " + string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("❌ Admin seeding error: " + ex.Message);
        }
    }
}
