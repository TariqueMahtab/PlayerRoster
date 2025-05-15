using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlayerRoster.Server.Data;
using PlayerRoster.Server.Data.Models;
using PlayerRoster.Server.DTOs;


namespace PlayerRoster.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PlayersController : ControllerBase
    {
        private readonly ApplicationDbContext _ctx;

        public PlayersController(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Player>>> GetAll()
        {
            var players = await _ctx.Players.Include(p => p.Team).ToListAsync();
            return Ok(players);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Player>> GetById(int id)
        {
            var player = await _ctx.Players.Include(p => p.Team).FirstOrDefaultAsync(p => p.Id == id);
            if (player == null) return NotFound();
            return Ok(player);
        }

        [HttpPost]
        public async Task<ActionResult<Player>> Create(PlayerCreateDto dto)
        {
            var team = await _ctx.Teams.FindAsync(dto.TeamId);
            if (team == null) return BadRequest($"Team with ID {dto.TeamId} does not exist.");

            var player = new Player
            {
                FullName = dto.FullName,
                Position = dto.Position,
                PPG = (float)dto.Ppg,
                RPG = (float)dto.Rpg,
                APG = (float)dto.Apg,
                TeamId = dto.TeamId
            };

            _ctx.Players.Add(player);
            await _ctx.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = player.Id }, player);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, PlayerCreateDto dto)
        {
            var player = await _ctx.Players.FindAsync(id);
            if (player == null) return NotFound();

            var team = await _ctx.Teams.FindAsync(dto.TeamId);
            if (team == null) return BadRequest($"Team with ID {dto.TeamId} does not exist.");

            player.FullName = dto.FullName;
            player.Position = dto.Position;
            player.PPG = (float)dto.Ppg;
            player.RPG = (float)dto.Rpg;
            player.APG = (float)dto.Apg;
            player.TeamId = dto.TeamId;

            await _ctx.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var player = await _ctx.Players.FindAsync(id);
            if (player == null) return NotFound();

            _ctx.Players.Remove(player);
            await _ctx.SaveChangesAsync();

            return NoContent();
        }
    }
}
