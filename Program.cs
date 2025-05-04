using API_Usuarios.Data;
using API_Usuarios.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace API_Usuarios
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Configuración de la base de datos
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseMySql(
                    builder.Configuration.GetConnectionString("DefaultConnection"),
                    ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
                ));

            // Configuración de la autenticación JWT
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = "API_Usuarios",
                        ValidAudience = "API_Usuarios_Clientes",
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("tu_clave_secreta_aqui"))
                    };
                });

            builder.Services.AddScoped<JwtHelper>();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowLocalApps",
                    policy =>
                    {
                        policy.WithOrigins("http://127.0.0.1:8000", "http://localhost:3000")
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
            });

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();


            app.UseCors("AllowLocalApps");




            app.UseAuthentication();  // No olvides agregar esta línea para usar autenticación JWT
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
