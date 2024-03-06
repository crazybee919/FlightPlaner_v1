using Microsoft.EntityFrameworkCore;
using FlightPlaner.Models;

namespace FlightPlaner
{
    public class FlightPlannerDBContext : DbContext
    {
        public FlightPlannerDBContext(DbContextOptions<FlightPlannerDBContext> options) : base(options)
        {
            
        }
        public DbSet<Flight> Flights { get; set; }
        public DbSet<Airport> Airports { get; set; }
    }
}