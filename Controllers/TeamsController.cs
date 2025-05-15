using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlayerRoster.Server.Data.Models;

namespace PlayerRoster.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TeamsController : ControllerBase
    {
        private readonly ApplicationDbContext _ctx;
        public TeamsController(ApplicationDbContext ctx) => _ctx = ctx;

        // GET /api/Teams
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var teams = await _ctx.Teams.ToListAsync();
            return Ok(teams);
        }

        // GET /api/Teams/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var team = await _ctx.Teams.FindAsync(id);
            if (team == null) return NotFound();
            return Ok(team);
        }
    }
}
