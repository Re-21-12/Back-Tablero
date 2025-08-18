using Microsoft.EntityFrameworkCore;
using tablero_api.Data;
using tablero_api.Repositories;

namespace tablero_api
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddScoped<LocalidadRepository>();



            // EF Core + SQL Server
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

           
            // Dependencias

            /*
            builder.Services.AddScoped<ITableroRepository, Repository>();
            builder.Services.AddScoped<ITableroService, TableroService>();
            */

            // CORS
            var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? [];
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                {
                    builder.WithOrigins(allowedOrigins)
                           .AllowAnyHeader()
                           .AllowAnyMethod();
                });
            });


            var app = builder.Build();

            app.MapGet("/", () => "API funcionando");
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors("CorsPolicy");

            app.MapControllers();
           
            app.Run();
        }
    }
}