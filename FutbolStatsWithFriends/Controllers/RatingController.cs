using FutbolStatsWithFriends.Data;
using FutbolStatsWithFriends.DTOs.Rating;
using FutbolStatsWithFriends.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FutbolStatsWithFriends.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class RatingController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public RatingController(ApplicationDbContext context)
        {
            _context = context;
        }
        // GET: api/<RatingController>
        [HttpGet("player/{playerId}")]
        public async Task<ActionResult<IEnumerable<RatingReadDTO>>> GetByPlayerId(int playerId)
        {
            var playerExists = await _context.Players.AnyAsync(p => p.Id == playerId);
            if (!playerExists)
            {
                return NotFound(new ApiResponseFormat<Object>("Player does not exist", succeeded: false));
            }

            var ratings = await _context.Ratings
                 .Where(r => r.PlayerId == playerId)
                 .Select(r => new RatingReadDTO
                 {
                     Id = r.Id,
                     Speed = r.Speed,
                     Shooting = r.Shooting,
                     Passing = r.Passing,
                     Dribbling = r.Dribbling,
                     Defending = r.Defending,
                     Physicality = r.Defending,
                     Strength = r.Strength,
                     Goalkeeping = r.Goalkeeping,
                     UserId = r.UserId,
                     PlayerId = r.PlayerId,
                     PlayerName = r.Player.Name,
                 }).ToListAsync();

            return Ok(new ApiResponseFormat<IEnumerable<RatingReadDTO>>(ratings, "Successfull Search"));
        }

        // GET api/<RatingController>/5
        [HttpGet("player/{id}/own")]
        public async Task<ActionResult<RatingReadDTO>> GetOwnRatingsForPlayer(int id)
        {
            var usuarioIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(usuarioIdClaim))
            {
                return Unauthorized(new ApiResponseFormat<Object>("User not allowed to rate", succeeded: false));
            }

            int authorizedUserId = int.Parse(usuarioIdClaim);

            var rating = await _context.Ratings
                .Where(u => u.UserId == authorizedUserId && u.PlayerId == id)
                .Select(r => new RatingReadDTO
                {

                    Id = r.Id,
                    Speed = r.Speed,
                    Shooting = r.Shooting,
                    Passing = r.Passing,
                    Dribbling = r.Dribbling,
                    Defending = r.Defending,
                    Physicality = r.Physicality,
                    Strength = r.Strength,
                    Goalkeeping = r.Goalkeeping,
                    UserId = r.UserId,
                    PlayerId = r.PlayerId,
                    PlayerName = r.Player.Name
                }).FirstOrDefaultAsync();

            if (rating == null)
            {
                return NotFound(new ApiResponseFormat<Object>("You have not rate this player yet, or the player does not exist.", succeeded: false));
            }

            return Ok(rating);
        }

        // POST api/<RatingController>
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] RatingCreateDTO ratingCreateDTO)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized(new ApiResponseFormat<Object>("User not allowed to rate", succeeded: false));
            }

            int authorizedUserId = int.Parse(userIdClaim);

            //se analiza el rol que tiene el usuario si es Admin se salta la verificacion
            var isAdmin = User.IsInRole("Admin") || User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value == "Admin";
            if (!isAdmin)
            {
                var lastRating = await _context.Ratings
                    .Where(r => r.UserId == authorizedUserId)
                    .OrderByDescending(r => r.CreatedAt)
                    .FirstOrDefaultAsync();

                if (lastRating != null)
                {
                    //se verifica el numero de dias desde el ultimo rating creado si es menor a 15 dias no se permite aun calificar
                    var daysSinceLastRating = (DateTime.UtcNow - lastRating.CreatedAt).TotalDays;

                    if (daysSinceLastRating < 15)
                    {
                        int diasRestantes = 15 - (int)daysSinceLastRating;
                        return BadRequest(new ApiResponseFormat<Object>(
                            $"Debes esperar {diasRestantes} días más para volver a calificar a este jugador.",
                            succeeded: false));
                    }
                }
            }

            var playerFound = await _context.Players.FirstOrDefaultAsync(p => p.Id == ratingCreateDTO.PlayerId);
            if (playerFound == null)
            {
                return NotFound(new ApiResponseFormat<Object>($"Player does not exist.", succeeded: false));
            }

            var newRating = new Rating
            {
                Speed = ratingCreateDTO.Speed,
                Shooting = ratingCreateDTO.Shooting,
                Passing = ratingCreateDTO.Passing,
                Dribbling = ratingCreateDTO.Dribbling,
                Defending = ratingCreateDTO.Defending,
                Physicality = ratingCreateDTO.Physicality,
                Strength = ratingCreateDTO.Strength,
                Goalkeeping = ratingCreateDTO.Goalkeeping,
                UserId = authorizedUserId,
                PlayerId = ratingCreateDTO.PlayerId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Ratings.Add(newRating);
            await _context.SaveChangesAsync();
            return Ok(new ApiResponseFormat<Object>($"Rating for {playerFound.Name} was registered successfully.", succeeded: true));
        }

        // PUT api/<RatingController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] RatingUpdateDTO ratingUpdateDTO)
        {
            var usuarioIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(usuarioIdClaim))
            {
                return Unauthorized(new ApiResponseFormat<Object>("User not allowed to rate", succeeded: false));
            }
            int authorizedUserId = int.Parse(usuarioIdClaim);

            var rating = await _context.Ratings.FirstOrDefaultAsync(r => r.Id == id);
            if (rating == null)
            {
                return NotFound("Rating not found.");
            }

            if (rating.UserId != authorizedUserId)
            {
                return Unauthorized(new ApiResponseFormat<Object>("You can only edit your own ratings", succeeded: false));
            }

            rating.Speed = ratingUpdateDTO.Speed;
            rating.Shooting = ratingUpdateDTO.Shooting;
            rating.Passing = ratingUpdateDTO.Passing;
            rating.Dribbling = ratingUpdateDTO.Dribbling;
            rating.Defending = ratingUpdateDTO.Defending;
            rating.Physicality = ratingUpdateDTO.Physicality;
            rating.Strength = ratingUpdateDTO.Strength;
            rating.Goalkeeping = ratingUpdateDTO.Goalkeeping;

            await _context.SaveChangesAsync();

            var playerName = await _context.Players
        .Where(p => p.Id == rating.PlayerId)
        .Select(p => p.Name) // ◄── SQL solo descarga el texto del nombre, nada más.
        .FirstOrDefaultAsync();

            return Ok(new ApiResponseFormat<Object>($"Rating for {playerName} has been Updated successfully", succeeded: true));
        }

        // DELETE api/<RatingController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var usuarioIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(usuarioIdClaim))
            {
                return Unauthorized(new ApiResponseFormat<Object>("User not allowed to delete this rating.", succeeded: false));
            }

            int authorizedUserId = int.Parse(usuarioIdClaim);

            var rating = await _context.Ratings.FirstOrDefaultAsync(r => r.Id == id);
            if (rating == null)
            {
                return NotFound(new ApiResponseFormat<Object>("Rating not found.", succeeded: false));
            }

            if (rating.UserId != authorizedUserId)
            {
                return Unauthorized(new ApiResponseFormat<Object>("You can only delete your own ratings.", succeeded: false));
            }

            _context.Ratings.Remove(rating);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponseFormat<Object>("The rating has been deleted.", succeeded: true));
        }
    }
}
