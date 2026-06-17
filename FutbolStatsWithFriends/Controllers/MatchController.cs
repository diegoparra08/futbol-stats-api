using FutbolStatsWithFriends.Data;
using FutbolStatsWithFriends.DTOs.Goal;
using FutbolStatsWithFriends.DTOs.Match;
using FutbolStatsWithFriends.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FutbolStatsWithFriends.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MatchController : ControllerBase
    {
        public readonly ApplicationDbContext _context;

        public MatchController(ApplicationDbContext context)
        {
            _context = context;
        }
        // GET: api/<MatchController>
        [HttpGet]
        public async Task<ActionResult<ApiResponseFormat<IEnumerable<MatchReadDTO>>>> Get([FromQuery] int? year)
        {
            var query = _context.Matches.AsQueryable();

            if (year.HasValue)
            {
                query = query.Where(m => m.MatchDate.Year == year);
            }

           var matches = await query.Include(md => md.MatchDetails)
                .Include(g => g.Goals)
                .Select(m => new MatchReadDTO
                {
                    Id = m.Id,
                    MatchDate = m.MatchDate,
                    Location = m.Location,
                    Status = m.Status.ToString(),
                    TeamAScore = m.TeamAScore,
                    TeamBScore = m.TeamBScore,

                    MatchDetails = m.MatchDetails.Select(md => new MatchDetailReadDto
                    {
                        PlayerId = md.PlayerId,
                        PlayerName = md.Player != null ? md.Player.Name : "Unknown",
                        Team = md.Team.ToString(),
                        Recoveries = md.Recoveries,
                        Tackles = md.Tackles,
                        FoulsCommitted = md.FoulsCommitted

                    }).ToList(),

                    Goals = m.Goals.Select(g => new GoalReadDTO
                    {
                        Id = g.Id,
                        PlayerName = g.Player != null ? g.Player.Name : "Unknown",
                        Minute = g.Minute,
                        IsPenalty = g.IsPenalty,
                        IsFreeKick = g.IsFreeKick,

                    }).ToList()
                }).ToListAsync();
            
            return Ok(new ApiResponseFormat<IEnumerable<MatchReadDTO>>(matches, "Goals loaded successfully"));
        }

        // GET api/<MatchController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MatchReadDTO>> GetMatchById(int id)
        {//falta agregar el estado
            var match = await _context.Matches
                .Include(md => md.MatchDetails)
                .Include(g => g.Goals)
                .Where(m => m.Id == id)
                .Select(m => new MatchReadDTO
                {
                    Id = m.Id,
                    MatchDate = m.MatchDate,
                    Location = m.Location,
                    Status = m.Status.ToString(),
                    TeamAScore = m.TeamAScore,
                    TeamBScore = m.TeamBScore,

                    MatchDetails = m.MatchDetails.Select(md => new MatchDetailReadDto
                    {
                        PlayerId = md.PlayerId,
                        PlayerName = md.Player != null ? md.Player.Name : "Unknown",
                        Team = md.Team.ToString(),
                        Recoveries = md.Recoveries,
                        Tackles = md.Tackles,
                        FoulsCommitted = md.FoulsCommitted

                    }).ToList(),

                    Goals = m.Goals.Select(g => new GoalReadDTO
                    {
                        Id = g.Id,
                        PlayerName = g.Player != null ? g.Player.Name : "Unknown",
                        Minute = g.Minute,
                        IsPenalty = g.IsPenalty,
                        IsFreeKick = g.IsFreeKick,

                    }).ToList()

                }).FirstOrDefaultAsync();

            if (match == null)
            {
                return NotFound(new ApiResponseFormat<object>("The match does not exist.", false));
            }

            return Ok(new ApiResponseFormat<MatchReadDTO>(match, "Match details loaded successfully."));
        }

        // POST api/<MatchController>
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] MatchSaveDTO matchSaveDTO)
        {
            var newMatch = new Match
            {
                MatchDate = matchSaveDTO.MatchDate,
                Location = matchSaveDTO.Location,
                TeamAScore = 0,
                TeamBScore = 0,
            };

            foreach (var detailDTO in matchSaveDTO.MatchDetails)
            {
                var playerExists = await _context.Players.AnyAsync(p => p.Id == detailDTO.PlayerId);
                if (!playerExists)
                {
                    return NotFound(new ApiResponseFormat<object>("The player does not exist.", false));
                }

                var detail = new MatchDetail
                {
                    PlayerId = detailDTO.PlayerId,
                    Team = detailDTO.Team
                };

                newMatch.MatchDetails.Add(detail);
            }

            _context.Matches.Add(newMatch);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMatchById), new { id = newMatch.Id }, "Match and lineup created successfully."); //cambiar para entregar el id
        }

        // PUT api/<MatchController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateMatchBasicInfo(int id, [FromBody] MatchUpdateDTO matchUpdateDTO)
        {
            var match = await _context.Matches.FirstOrDefaultAsync(m => m.Id == id);
            if (match == null)
            {
                return NotFound(new ApiResponseFormat<object>("The match does not exist.", false));
            }

            match.MatchDate = matchUpdateDTO.MatchDate;
            match.Location = matchUpdateDTO.Location;

            await _context.SaveChangesAsync();
            return Ok(new ApiResponseFormat<object>("Match basic info updated successfully", true));
        }

        [HttpPut("{id}/teams&stats")]
        public async Task<IActionResult> UpdateMatchPlayerStats(int id, [FromBody] MatchStatsUpdateDTO matchStatsUpdateDto)
        {
            // Buscamos los detalles de la alineación de este partido
            var matchDetails = await _context.MatchDetails
                .Where(md => md.MatchId == id)
                .ToListAsync();

            if (!matchDetails.Any())
            {
                return NotFound(new ApiResponseFormat<object>("No lineups found for this match", false));
            }

            //Aca se recorre la lista de lo que trae el dto para extraer estadisticas
            foreach (var playerStat in matchStatsUpdateDto.PlayersStats)
            {
                var detail = matchDetails.FirstOrDefault(md => md.PlayerId == playerStat.PlayerId);

                if (detail != null)
                {
                    // Solo cambia de equipo si viene con valor. Sino se queda en el de la creacion
                    if (playerStat.Team.HasValue)
                    {
                        detail.Team = playerStat.Team.Value;
                    }

                    // Las estadísticas defensivas se sobrescriben con los nuevos valores de los contadores
                    detail.Recoveries = playerStat.Recoveries;
                    detail.Tackles = playerStat.Tackles;
                    detail.FoulsCommitted = playerStat.FoulsCommitted;
                }
            }

            await _context.SaveChangesAsync();
            return Ok(new ApiResponseFormat<object>("Statistics updated successfully", true));
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateMatchStatus(int id, [FromBody] int status)
        {
            var match = await _context.Matches.FirstOrDefaultAsync(m => m.Id == id);

            if (match == null)
            {
                return NotFound(new ApiResponseFormat<object>("The match does not exist.", false));
            }
            //Si se cancela se cambia el estado a cancelado
            if (status == 0)
            {
                match.Status = MatchStatus.Scheduled;
            }
            else if (status == 1)
            {
                match.Status = MatchStatus.InPlay;
            }
            else if (status == 2)
            {
                match.Status = MatchStatus.Finished;
            }
            else if (status == 3)
            {
                match.Status = MatchStatus.Cancelled;
            }

            await _context.SaveChangesAsync();

            return Ok(new ApiResponseFormat<object>("Match status updated.", true));
        }

        // DELETE api/<MatchController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
