using System.Reflection;
using FlightPlanner.Core.Models;
using FlightPlanner.KautkKas;
using FlightPlanner.Services;
using FlightPlanner.UseCases;
using FlightPlannerCore.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;

namespace FlightPlanner
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();

            // configure basic authentication 
            builder.Services.AddAuthentication("BasicAuthentication")
                .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);


            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            
            builder.Services.AddDbContextPool<FlightPlannerDBContext>( options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("flight-planner"));
            });
            builder.Services.AddTransient<IFlightPlannerDbContext, FlightPlannerDBContext>();
            builder.Services.AddTransient<IDbService, DbService>();
            builder.Services.AddTransient<IEntityService<Airport>, EntityService<Airport>>();
            builder.Services.AddTransient<IEntityService<Flight>, EntityService<Flight>>();
            builder.Services.AddTransient<IFlightService, FlightService>();
            var assembly = Assembly.GetExecutingAssembly();
            builder.Services.AddAutoMapper(assembly);
            builder.Services.AddValidatorsFromAssembly(assembly);
            builder.Services.AddServices();
            
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthentication();
            app.UseAuthorization(); 



            app.MapControllers();

            app.Run();
        }
    }
}
