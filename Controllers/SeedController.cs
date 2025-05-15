using CsvHelper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using PlayerRoster.Server.Data.Models;
using System.Globalization;

namespace PlayerRoster.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SeedController : ControllerBase
    {
        private readonly ApplicationDbContext _ctx;
        private readonly IWebHostEnvironment _env;
        private readonly IPasswordHasher<ApplicationUser> _passwordHasher;

        public SeedController(ApplicationDbContext ctx, IWebHostEnvironment env, IPasswordHasher<ApplicationUser> passwordHasher)
        {
            _ctx = ctx;
            _env = env;
            _passwordHasher = passwordHasher;
        }

        [HttpGet, HttpPost]
        public async Task<IActionResult> SeedFromCsv()
        {
            if (_ctx.Players.Any())
                return BadRequest("Players already seeded.");

            var filePath = Path.Combine(_env.ContentRootPath, "Data", "NbaPlayerStats.csv");

            using var reader = new StreamReader(filePath);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            var csvPlayers = csv.GetRecords<PlayerCsv>().ToList();

            var players = csvPlayers.Select(p => new Player
            {
                FullName = p.Name,
                Position = p.Position,
                PPG = p.Points,
                RPG = p.Rebounds,
                APG = p.Assists
            }).ToList();

            await _ctx.Players.AddRangeAsync(players);

            // Force recreate "comp" user
            var existingComp = _ctx.Users.FirstOrDefault(u => u.UserName.ToLower() == "comp");
            if (existingComp != null)
                _ctx.Users.Remove(existingComp);

            var compUser = new ApplicationUser
            {
                UserName = "comp",
                Email = "comp@example.com",
                NormalizedUserName = "COMP",
                NormalizedEmail = "COMP@EXAMPLE.COM",
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString()
            };
            compUser.PasswordHash = _passwordHasher.HashPassword(compUser, "Comp584");
            _ctx.Users.Add(compUser);

            // Add "admin" user if not exists
            if (!_ctx.Users.Any(u => u.UserName == "admin"))
            {
                var adminUser = new ApplicationUser
                {
                    UserName = "admin",
                    Email = "admin@example.com",
                    NormalizedUserName = "ADMIN",
                    NormalizedEmail = "ADMIN@EXAMPLE.COM",
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString()
                };
                adminUser.PasswordHash = _passwordHasher.HashPassword(adminUser, "Admin123!");
                _ctx.Users.Add(adminUser);
            }

            await _ctx.SaveChangesAsync();

            return Ok("Seeding complete.");
        }
    }
}
