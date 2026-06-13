
using FutbolStatsWithFriends.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace FutbolStatsWithFriends
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowNextJS", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });
            // Add services to the container.
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new() { Title = "Futbol Stats With Friends", Version = "v1" });

                // 1. Definir el esquema de seguridad (JWT)
                c.AddSecurityDefinition("Bearer", new()
                {
                    Name = "Authorization",
                    //Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Description = "Introduce el token JWT de esta manera: Bearer {tu_token_aquí}"
                });

                // 2. Hacer que Swagger use ese esquema globalmente. Se puede quitar cuando se tenga el front levantado
                c.AddSecurityRequirement(new()
    {
        {
            new()
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme, // <-- ESTA ES LA CLASE REAL
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
            });

            var jwtKey = builder.Configuration["Jwt:Key"]
                ?? throw new InvalidOperationException("La clave JWT no está configurada.");
            var jwtIssuer = builder.Configuration["Jwt:Issuer"];
            var jwtAudience = builder.Configuration["Jwt:Audience"];

            // 2. Le decimos a .NET que use autenticación por Tokens (JwtBearer)
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true, // Verifica que el token no haya expirado
                    ValidateIssuerSigningKey = true, // Valida que la firma sea legítima
                    ValidIssuer = jwtIssuer,
                    ValidAudience = jwtAudience,
                    // Convertimos la frase secreta string en bytes criptográficos
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                };
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseRouting();
            app.UseCors("AllowNextJS");

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
