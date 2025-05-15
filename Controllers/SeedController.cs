using ExcelDataReader;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using PlayerRoster.Server.Data;         
using PlayerRoster.Server.Data.Models;   

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

        [HttpPost]
        public IActionResult Seed()
        {
            // 1) Open and read Excel with the first row as headers
            var filePath = Path.Combine(_env.ContentRootPath, "Data", "NbaPlayerStats.xlsx");
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            using var stream = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read);
            using var reader = ExcelReaderFactory.CreateReader(stream);
            var conf = new ExcelDataSetConfiguration
            {
                ConfigureDataTable = _ => new ExcelDataTableConfiguration
                {
                    UseHeaderRow = true
                }
            };

            var table = reader.AsDataSet(conf).Tables[0];

            // 2) Clear out existing data
            _ctx.Players.RemoveRange(_ctx.Players);
            _ctx.Teams.RemoveRange(_ctx.Teams);
            _ctx.SaveChanges();

            // 3) Seed Teams based on "City" column
            var teams = new Dictionary<string, Team>();
            foreach (DataRow row in table.Rows)
            {
                var city = row["City"]?.ToString()?.Trim();
                if (string.IsNullOrEmpty(city) || teams.ContainsKey(city))
                    continue;

                var team = new Team { Name = city, City = city };
                teams[city] = team;
                _ctx.Teams.Add(team);
            }
            _ctx.SaveChanges();

            // 4) Helper to parse floats safely
            static float GetFloat(object? cell)
            {
                var txt = cell?.ToString()?.Trim();
                return float.TryParse(txt, out var f) ? f : 0f;
            }

            // 5) Seed Players from the remaining columns
            foreach (DataRow row in table.Rows)
            {
                var city = row["City"]?.ToString()?.Trim();
                if (string.IsNullOrEmpty(city) || !teams.ContainsKey(city))
                    continue;

                _ctx.Players.Add(new Player
                {
                    FullName = row["Name"]?.ToString()?.Trim() ?? "",
                    Position = row["Position"]?.ToString()?.Trim() ?? "",
                    PPG = GetFloat(row["Points"]),
                    RPG = GetFloat(row["Rebounds"]),
                    APG = GetFloat(row["Assists"]),
                    TeamId = teams[city].Id
                });
            }
            _ctx.SaveChanges();

            return Ok("Seeding complete");
        }
    }
}

