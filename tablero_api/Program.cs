using Microsoft.EntityFrameworkCore;
using tablero_api.Data;
using tablero_api.Repositories;
using tablero_api.Services;
using tablero_api.Services.Interfaces;

namespace tablero_api
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
            });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddScoped<LocalidadRepository>();



            // EF Core + SQL Server
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


            // Dependencias


            builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            builder.Services.AddScoped(typeof(IService<>), typeof(Service<>));


            // CORS
            var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? [];
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy.WithOrigins(
                        "http://localhost:4200",
                        "https://front-analisis-registros.netlify.app",
                        "https://proy-analisis-re2112.duckdns.org",
                        "http://frontend:4200",
                        "http://157.180.19.137:4200",
                        "http://157.180.19.137:80"

                    )
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

            app.UseCors("AllowFrontend");

            app.MapControllers();
           
            app.Run();

            



        }
    }
}