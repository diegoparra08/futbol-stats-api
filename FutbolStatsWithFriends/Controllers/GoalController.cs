using FutbolStatsWithFriends.Data;
using FutbolStatsWithFriends.DTOs.Goal;
using FutbolStatsWithFriends.DTOs.Player;
using FutbolStatsWithFriends.DTOs.Rating;
using FutbolStatsWithFriends.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FutbolStatsWithFriends.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GoalController : ControllerBase
    {
        public readonly ApplicationDbContext _context;

        public GoalController(ApplicationDbContext context)
        {
            _context = context;
        }
        // GET: api/<ValuesController>
        [HttpGet]
        public async Task<ActionResult<ApiResponseFormat<IEnumerable<GoalReadDTO>>>> GetGoals(
        [FromQuery] int? matchId,
        [FromQuery] int? playerId,
        [FromQuery] int? year)
        {
            //Aca se prerpara el query con AsQueryale y se analiza que metodos tienen
            var query = _context.Goals.AsQueryable();
            //Se agrega el filtro si es que viene y se van sumando todos los filtros que vengan
            if (matchId.HasValue)
            {
                query = query.Where(g => g.MatchId == matchId.Value);
            }
            if (playerId.HasValue)
            {
                query = query.Where(g => g.PlayerId == playerId.Value);
            }
            if (year.HasValue)
            {
                query = query.Where(g => g.Match.MatchDate.Year == year);
            }

            var goals = await query
                .Select(g => new GoalReadDTO
                {
                    Id = g.Id,
                    Minute = g.Minute,
                    MatchId = g.MatchId,
                    PlayerId = g.PlayerId,
                    PlayerName = g.Player.Name,
                    AssistedByPlayerId = g.AssistedByPlayerId,
                    AssistedByPlayerName = g.AssistedByPlayer != null ? g.AssistedByPlayer.Name : null,
                    IsPenalty = g.IsPenalty,
                    IsFreeKick = g.IsFreeKick,
                    MatchDate = g.Match.MatchDate
                })
                .ToListAsync();
            return Ok(new ApiResponseFormat<IEnumerable<GoalReadDTO>>(goals, "Goals loaded successfully"));
        }

        // GET: api/<ValuesController>/5

        [HttpGet("{id}")]

        public async Task<ActionResult<GoalReadDTO>> Get(int id)
        {
            var goal = await _context.Goals.
                Where(g => g.Id == id).
                Select(g => new GoalReadDTO
                {
                    Id = g.Id,
                    Minute = g.Minute,
                    MatchId = g.MatchId,
                    PlayerId = g.PlayerId,
                    PlayerName = g.Player.Name,
                    AssistedByPlayerId = g.AssistedByPlayerId,
                    AssistedByPlayerName = g.AssistedByPlayer != null ? g.AssistedByPlayer.Name : null,
                    IsPenalty = g.IsPenalty,
                    IsFreeKick = g.IsFreeKick,
                    MatchDate = g.Match.MatchDate
                }).FirstOrDefaultAsync();

            if (goal == null)
            {
                return NotFound(new ApiResponseFormat<Object>("Goal does not exist.", succeeded: false));
            }

            return Ok(new ApiResponseFormat<GoalReadDTO>(goal, "Player Found Successfully"));
            //}

            // POST api/<ValuesController>
            [HttpPost]
        public async Task<ActionResult> Post([FromBody] GoalCreateDTO goalCreateDTO)
        {
           
            var match = await _context.Matches.FindAsync(goalCreateDTO.MatchId);
            if (match == null)
            {
                return NotFound(new ApiResponseFormat<object>("The match does not exist.", false));
            }

            //Aca se valida que el jugador exista y que si este jugando en el partido (que pertenece a alguno de los equipos)
            var playerMatchDetail = await _context.MatchDetails
                .FirstOrDefaultAsync(md => md.MatchId == goalCreateDTO.MatchId && md.PlayerId == goalCreateDTO.PlayerId);

            if (playerMatchDetail == null)
            {
                return NotFound(new ApiResponseFormat<object>("The scoring player is not registered or playing in this match.", false));
            }
            //Verificacion de si hay assitidor
            if (goalCreateDTO.AssistedByPlayerId.HasValue)
            {
                //vemos si el mismo anotador es el mismo asistidor porque no es posible
                if (goalCreateDTO.AssistedByPlayerId == goalCreateDTO.PlayerId)
                {
                    return BadRequest(new ApiResponseFormat<object>("A player cannot assist their own goal.", false));
                }
                //verificamos que si exista el asistidor en la DB
                var assistantExists = await _context.Players.AnyAsync(p => p.Id == goalCreateDTO.AssistedByPlayerId.Value);
                if (!assistantExists)
                {
                    return NotFound(new ApiResponseFormat<object>("The assistant player does not exist.", false));
                }
            }

            var newGoal = new Goal
            {
                MatchId = goalCreateDTO.MatchId,
                Minute = goalCreateDTO.Minute,
                PlayerId = goalCreateDTO.PlayerId,
                IsPenalty = goalCreateDTO.IsPenalty.Value,
                IsFreeKick = goalCreateDTO.IsFreeKick.Value,
                AssistedByPlayerId = goalCreateDTO.AssistedByPlayerId,
            };

            //Se evalua el team del jugador y se suma un punto al marcador del equipo
            if (playerMatchDetail.Team == TeamSide.TeamA) match.TeamAScore++;
            else if (playerMatchDetail.Team == TeamSide.TeamB) match.TeamBScore++;

            _context.Add(newGoal);
            await _context.SaveChangesAsync();


            return CreatedAtAction(nameof(GetGoals), new { id = newGoal.Id }, new ApiResponseFormat<object>(newGoal.Id ,"Goal recorded successfully."));
        }

        // PUT api/<ValuesController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] GoalUpdateDTO goalUpdateDTO)
        {
            var goalExixts = await _context.Goals.FirstOrDefaultAsync(g => g.Id == id);
            if (goalExixts == null)
            {
                return NotFound(new ApiResponseFormat<Object>($"Goal does not exist", false));
            }

            goalExixts.Minute = goalUpdateDTO.Minute;
            goalExixts.PlayerId = goalUpdateDTO.PlayerId;
            goalExixts.IsFreeKick = goalUpdateDTO.IsFreeKick.Value;  
            goalExixts.IsPenalty = goalUpdateDTO.IsPenalty.Value;
            goalExixts.AssistedByPlayerId = goalUpdateDTO.AssistedByPlayerId;
           

            await _context.SaveChangesAsync();
            return Ok(new ApiResponseFormat<object>($"Goal details have been Updated successfully", true));
        }

        // DELETE api/<ValuesController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            //Aca se borra pero tambien se debe restar el gol del partido.
            //Primero se busca el gol y se trae el partido en el include
            var goal = await _context.Goals
                    .Include(g => g.Match)
                    .FirstOrDefaultAsync(g => g.Id == id);

            if (goal == null)
            {
                return NotFound(new ApiResponseFormat<object>("Goal not found", false));
            }

            //Se busca el jugador en MatchDetails con del gol para poder identificar el Team al que pertenecia
            var playerMatchDetail = await _context.MatchDetails
                    .FirstOrDefaultAsync(md => md.MatchId == goal.MatchId && md.PlayerId == goal.PlayerId);

            if (playerMatchDetail != null)
            {
                // Restar el gol del marcador global según su equipo
                if (playerMatchDetail.Team == TeamSide.TeamA)
                {
                    if (goal.Match.TeamAScore > 0) goal.Match.TeamAScore--;
                }
                else if (playerMatchDetail.Team == TeamSide.TeamB)
                {
                    if (goal.Match.TeamBScore > 0) goal.Match.TeamBScore--;
                }
            }

            _context.Remove(goal);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponseFormat<object>("Goal deleted and match score updated successfully", true));
        }
    }
}
