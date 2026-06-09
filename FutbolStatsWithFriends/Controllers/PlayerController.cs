using FutbolStatsWithFriends.Data;
using FutbolStatsWithFriends.DTOs.Player;
using FutbolStatsWithFriends.DTOs.PlayerPositionDTO;
using FutbolStatsWithFriends.DTOs.Rating;
using FutbolStatsWithFriends.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Validations;
using System.Numerics;
using System.Threading.Tasks.Dataflow;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FutbolStatsWithFriends.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PlayerController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PlayerController(ApplicationDbContext context)
        {
            _context = context;
        }
        // GET: api/<PlayerController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlayerReadDTO>>> GetAll([FromQuery] bool includeInactive = false)
        {
            var query = _context.Players.AsQueryable();

            if (!includeInactive)
            {
                query = query.Where(p => p.IsActive == true);
            }

            var players = await query
         .Select(p => new PlayerReadDTO
         {
             Id = p.Id,
             Name = p.Name,
             Nickname = p.Nickname,
             PhotoUrl = p.PhotoUrl,
             PreferredFoot = p.PreferredFoot,
             IsActive = p.IsActive,
             OverallRating = p.Ratings.Any()
                    ? (p.Positions.Any(pos => pos.PositionName == PlayerPosition.Positions.GK)
                // SI ES ARQUERO: Promediamos Goalkeeping, Físico y Pase (División entre 3)
                ? (p.Ratings.Average(r => r.Goalkeeping) + p.Ratings.Average(r => r.Physicality) + p.Ratings.Average(r => r.Passing)) / 3
                // SI ES JUGADOR DE CAMPO: Ignoramos Goalkeeping por completo (División entre 6)
                : (p.Ratings.Average(r => r.Speed) + p.Ratings.Average(r => r.Shooting) + p.Ratings.Average(r => r.Passing) + p.Ratings.Average(r => r.Dribbling) + p.Ratings.Average(r => r.Defending) + p.Ratings.Average(r => r.Physicality)) / 6)
            : 0,
             Age = p.Age,
             Height = p.Height,

             Positions = p.Positions.Select(pos => pos.PositionName.ToString()).ToList(),

             AvgSpeed = p.Ratings.Any() ? p.Ratings.Average(r => r.Speed) : 0,
             AvgShooting = p.Ratings.Any() ? p.Ratings.Average(r => r.Shooting) : 0,
             AvgPassing = p.Ratings.Any() ? p.Ratings.Average(r => r.Passing) : 0,
             AvgDribbling = p.Ratings.Any() ? p.Ratings.Average(r => r.Dribbling) : 0,
             AvgDefending = p.Ratings.Any() ? p.Ratings.Average(r => r.Defending) : 0,
             AvgPhysicality = p.Ratings.Any() ? p.Ratings.Average(r => r.Physicality) : 0,
             AvgStrength = p.Ratings.Any() ? p.Ratings.Average(r => r.Strength) : 0,
             AvgGoalkeeping = p.Ratings.Any() ? p.Ratings.Average(r => r.Goalkeeping) : 0
         }).ToListAsync();

            return Ok(new ApiResponseFormat<IEnumerable<PlayerReadDTO>>(players, "Successfull Search"));
        }

        // GET api/<PlayerController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PlayerReadDTO>> GetById(int id)
        {
            var player = await _context.Players
                 .Where(p => p.Id == id)
                 .Select(p => new PlayerReadDTO
                 {
                     Id = p.Id,
                     Name = p.Name,
                     Nickname = p.Nickname,
                     PhotoUrl = p.PhotoUrl,
                     PreferredFoot = p.PreferredFoot,
                     IsActive = p.IsActive,

                     OverallRating = p.Ratings.Any()
                     ? (p.Positions.Any(pos => pos.PositionName == PlayerPosition.Positions.GK)
                // SI ES ARQUERO: Promediamos Goalkeeping, Físico y Pase (División entre 3)
                ? (p.Ratings.Average(r => r.Goalkeeping) + p.Ratings.Average(r => r.Physicality) + p.Ratings.Average(r => r.Passing)) / 3
                // SI ES JUGADOR DE CAMPO: Ignoramos Goalkeeping por completo (División entre 6)
                : (p.Ratings.Average(r => r.Speed) + p.Ratings.Average(r => r.Shooting) + p.Ratings.Average(r => r.Passing) + p.Ratings.Average(r => r.Dribbling) + p.Ratings.Average(r => r.Defending) + p.Ratings.Average(r => r.Physicality)) / 6)
            : 0,
                     Age = p.Age,
                     Height = p.Height,

                     Positions = p.Positions.Select(pos => pos.PositionName.ToString()).ToList(),

                     AvgSpeed = p.Ratings.Any() ? p.Ratings.Average(r => r.Speed) : 0,
                     AvgShooting = p.Ratings.Any() ? p.Ratings.Average(r => r.Shooting) : 0,
                     AvgPassing = p.Ratings.Any() ? p.Ratings.Average(r => r.Passing) : 0,
                     AvgDribbling = p.Ratings.Any() ? p.Ratings.Average(r => r.Dribbling) : 0,
                     AvgDefending = p.Ratings.Any() ? p.Ratings.Average(r => r.Defending) : 0,
                     AvgPhysicality = p.Ratings.Any() ? p.Ratings.Average(r => r.Physicality) : 0,
                     AvgStrength = p.Ratings.Any() ? p.Ratings.Average(r => r.Strength) : 0,
                     AvgGoalkeeping = p.Ratings.Any() ? p.Ratings.Average(r => r.Goalkeeping) : 0

                 }).FirstOrDefaultAsync();

            return Ok(new ApiResponseFormat<PlayerReadDTO>(player, "Player Found Successfully"));
        }

        [HttpGet("{playerId}/stats")]
        public async Task<ActionResult<PlayerStatsReadDTO>> GetStats(int playerId)
        {
            var playerExists = await _context.Players
                .AnyAsync(p => p.Id == playerId);
            if (!playerExists)
            {
                return NotFound(new ApiResponseFormat<Object>("Player not Found", succeeded: false));
            }

            int totalGoals, totalAssists;

            int matchesPlayed = await _context.MatchDetails
                .CountAsync(md => md.PlayerId == playerId && md.Match.Status == MatchStatus.Finished);
            if (matchesPlayed <= 0)
            {
                totalGoals = 0;
                totalAssists = 0;

            }
            else
            {
                totalGoals = await _context.Goals
           .CountAsync(g => g.PlayerId == playerId && g.Match.Status == MatchStatus.Finished);

                totalAssists = await _context.Goals
                .CountAsync(g => g.AssistedByPlayerId == playerId && g.Match.Status == MatchStatus.Finished);
            }

            var stats = new PlayerStatsReadDTO
            {
                PlayerId = playerId,
                MatchesPlayed = matchesPlayed,
                Goals = totalGoals,
                Assists = totalAssists
            };

            return Ok(new ApiResponseFormat<PlayerStatsReadDTO>(stats, "Player Stats loaded successfully."));
        }

        // POST api/<PlayerController>
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] PlayerCreateDTO playerCreateDTO)
        {
            var newPlayer = new Player
            {
                Name = playerCreateDTO.Name,
                Nickname = playerCreateDTO.Nickname,
                PhotoUrl = playerCreateDTO.PhotoUrl,
                PreferredFoot = playerCreateDTO.PreferredFoot,
                Age = playerCreateDTO.Age,
                Height = playerCreateDTO.Height,

            };
            foreach (var posicionEnum in playerCreateDTO.Positions)
            {
                newPlayer.Positions.Add(new PlayerPosition
                {
                    PositionName = posicionEnum
                });
            }

            _context.Players.Add(newPlayer);
            await _context.SaveChangesAsync();

            var playerResultDto = new PlayerReadDTO
            {
                Id = newPlayer.Id,
                Name = newPlayer.Name
            };

            var apiResponse = new ApiResponseFormat<PlayerReadDTO>(playerResultDto, "Player added successfully");

            return CreatedAtAction(nameof(GetById), new { id = newPlayer.Id }, apiResponse);
        }

        // PUT api/<PlayerController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] PlayerUpdateDTO playerUpdateDTO)
        {
            var player = await _context.Players
                .Include(pos => pos.Positions)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (player == null)
            {
                return NotFound();
            }

            player.Name = playerUpdateDTO.Name;
            player.Nickname = playerUpdateDTO.Nickname;
            player.PhotoUrl = playerUpdateDTO.PhotoUrl;
            player.PreferredFoot = playerUpdateDTO.PreferredFoot;
            //Eliminar las posiciones existentes para que no se haga conflicto. 
            _context.PlayerPositions.RemoveRange(player.Positions);
            //Se verifica que no venga null el player positions y se agregan de nuevo las posiciones
            if (playerUpdateDTO.Positions != null && playerUpdateDTO.Positions.Any())
            {
                foreach (var positionEnum in playerUpdateDTO.Positions)
                {
                    player.Positions.Add(new PlayerPosition
                    {
                        PositionName = positionEnum
                    });
                }
            }

            await _context.SaveChangesAsync();

            return Ok(new ApiResponseFormat<Object>($"Player '{playerUpdateDTO.Name}' updated Successfully.", succeeded: true));
        }

        // DELETE api/<PlayerController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var player = await _context.Players.FindAsync(id);
            if (player == null)
            {
                return NotFound(new ApiResponseFormat<Object>("Player does not exist.", succeeded: false));
            }
            player.IsActive = false;

            await _context.SaveChangesAsync();

            return Ok(new ApiResponseFormat<Object>($"Player {player.Name} has been deactivated.", succeeded: true));
        }
    }
}
