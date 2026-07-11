using FutbolStatsWithFriends.DTOs;
using FutbolStatsWithFriends.Models;
using FutbolStatsWithFriends.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FutbolStatsWithFriends.DTOs.Auth;
using FutbolStatsWithFriends.DTOs.User;

namespace ColeccionablesCaros.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config; //Esto lee el appsettings.json y permite crear los tokens 

        // Inyectamos el contexto de la BD y la configuración para leer la Key Secreta
        public AuthController(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            // Validar si el correo ya existe para no duplicarlo
            var existeUsuario = await _context.Users.AnyAsync(u => u.Email.ToLower() == registerDto.Email.ToLower());
            if (existeUsuario)
            {
                return BadRequest("Email address is already registered.");
            }

            //Esta linea encripta la contraseña
            string contraseñaEncriptada = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

            // Creamos el nuevo usuario
            var newUser = new User
            {
                Email = registerDto.Email,
                PasswordHash = contraseñaEncriptada, // se guarda el Hash
                Name = registerDto.Name,
                Role = Roles.Player
            };

            _context.Users.Add(newUser);

            await _context.SaveChangesAsync();
            return Ok(new ApiResponseFormat<Object>($"User registered successfully", succeeded: true));
           
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginDTO>> Login([FromBody] LoginDTO loginDto)
        {
            // Buscar al usuario por su correo
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == loginDto.Email.ToLower());

            // comparar contraseñas. elverify pasa los valores a comparar y retorna bool
            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                return Unauthorized(new ApiResponseFormat<Object>($"Email or password incorrect", succeeded: false));
            }

            //Aca se empiza a formar el contenido del token dentro del [] claims
            // Los "Claims" son las características que van grabadas dentro del token, por ejemplo nombre, role, etc
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, user.Role.ToString()) //pasamos a string el role porque viene como un int de la DB
            };

            //Traemos la clave secreta desde el appsettings y la transformamos en bytes
            var jwtKey = _config["Jwt:Key"] ?? throw new InvalidOperationException("Key has not been defined.");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Creamos el objeto del Token con su tiempo de expiración (Ej: Expira en 1 día)
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                //Expires = DateTime.UtcNow.AddHours(1), Esto se configura mejor en el appsettings
                Issuer = _config["Jwt:Issuer"],
                Audience = _config["Jwt:Audience"],
                SigningCredentials = creds
            };

            //Generamos el string largo final (El token de texto)
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            //Le respondemos a Next.js con el DTO de éxito
            return Ok(new LoginResponseDTO
            {
                Token = tokenString,
                Email = user.Email,
                Name = user.Name
            });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] UserSaveDTO userSaveDTO)
        {
            var userFound = await _context.Users.FindAsync(id);
            if (userFound == null)
            {
                return NotFound("User not found.");
            }

            //las verificaciones hacen que solo se actualicen los campos que no estén vacíos, así no se borran datos si el usuario no quiere cambiar algo
            if (!string.IsNullOrWhiteSpace(userSaveDTO.Name))
            {
                userFound.Name = userSaveDTO.Name;
            }

            if (!string.IsNullOrWhiteSpace(userSaveDTO.Email))
            {
                userFound.Email = userSaveDTO.Email;
            }

            if (userSaveDTO.Role != null)
            {
                userFound.Role = userSaveDTO.Role;
            }

            if (!string.IsNullOrEmpty(userSaveDTO.Password))
            {
                userFound.PasswordHash = BCrypt.Net.BCrypt.HashPassword(userSaveDTO.Password);
            }

            await _context.SaveChangesAsync();

            return Ok(new ApiResponseFormat<Object>($"User {userFound.Name} has been updated successfully", succeeded: true));
        }
    }
}