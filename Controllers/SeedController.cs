using CsvHelper;
using Microsoft.AspNetCore.Mvc;
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

        public SeedController(ApplicationDbContext ctx, IWebHostEnvironment env)
        {
            _ctx = ctx;
            _env = env;
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
                APG = p.Assists,
              
            }).ToList();

            await _ctx.Players.AddRangeAsync(players);
            await _ctx.SaveChangesAsync();

            return Ok("CSV seeding complete.");
        }
    }
}
