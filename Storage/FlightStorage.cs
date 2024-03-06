using FlightPlaner.Models;

namespace FlightPlaner.Storage
{
    public class FlightStorage
    {
        private static List<Flight> _flights = new List<Flight>();
        private static int _id;
        
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
    }
}