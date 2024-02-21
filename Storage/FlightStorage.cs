using FlightPlaner.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Threading.Tasks;


namespace FlightPlaner.Storage
{
    public class FlightStorage
    {
        private static List<Flight> _flights = new List<Flight>();
        private static int _id;

        public static void AddFlight(Flight flight)
        {
            flight.Id = _id++;
            _flights.Add(flight);
        }

        public static void DeleteFlight(Flight flight)
        {
            _flights.Remove(flight);
        }

        public static void Clear()
        {
            _flights.Clear();
        }

        public static Flight GetFlightById(int id)
        {
            return _flights.FirstOrDefault(flight => flight.Id == id);
        }

        public static List<Flight> GetFlightByDest(string airportFrom, string airportTo, string date)
        {
            var ParsedDate = DateTime.Parse(date).Date;
            var list = new List<Flight>();
            list.AddRange(_flights.Where(flight =>
                flight.From.AirportCode == airportFrom && flight.To.AirportCode == airportTo &&
                DateTime.Parse(flight.DepartureTime).Date == ParsedDate));
            return list;
        }

        public static bool IsFlightValuesEmpty(Flight flight)
        {
            if (String.IsNullOrEmpty(flight.To.AirportCode) || String.IsNullOrEmpty(flight.From.AirportCode) ||
                String.IsNullOrEmpty(flight.Carrier) || String.IsNullOrEmpty(flight.ArrivalTime) ||
                String.IsNullOrEmpty(flight.DepartureTime))
            {
                return true;
            }

            return false;
        }

        public static bool IsFlightTimeCorrect(Flight flight)
        {
            if (DateTime.Parse(flight.ArrivalTime) > DateTime.Parse(flight.DepartureTime))
            {
                return true;
            }

            return false;
        }

        public static bool AreAirportsSame(Flight flight)
        {
            return flight.To.AirportCode.ToLower().Trim() == flight.From.AirportCode.ToLower().Trim();
        }

        public static bool DoesFlightExsist(Flight flight)
        {
            if (_flights.Any(x =>
                    x.Carrier == flight.Carrier &&
                    DateTime.Parse(x.ArrivalTime) == DateTime.Parse(flight.ArrivalTime) &&
                    DateTime.Parse(x.DepartureTime) == DateTime.Parse(flight.DepartureTime) &&
                    x.From.AirportCode == flight.From.AirportCode &&
                    x.To.AirportCode == flight.To.AirportCode &&
                    x.From.City == flight.From.City &&
                    x.From.Country == flight.From.Country))
            {
                return true;
            }

            return false;
        }
    }
}