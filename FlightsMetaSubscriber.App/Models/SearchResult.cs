using System.Text.Json.Serialization;

namespace FlightsMetaSubscriber.App.Models;

public class SearchResult
{
    public SearchResult(string originCityIata, string destinationCityIata, DateTimeOffset departureAt, double value, string ticketLink)
    {
        OriginCityIata = originCityIata;
        DestinationCityIata = destinationCityIata;
        DepartureAt = departureAt;
        Value = value;
        TicketLink = ticketLink;
    }

        [JsonPropertyName("origin_city_iata")]
        public string OriginCityIata { get; set; }

        [JsonPropertyName("destination_city_iata")]
        public string DestinationCityIata { get; set; }

        [JsonPropertyName("departure_at")]
        public DateTimeOffset DepartureAt { get; set; }

        [JsonPropertyName("value")]
        public double Value { get; set; }

        [JsonPropertyName("ticket_link")]
        public string TicketLink { get; set; }

        public override string ToString()
        {
                return $"Origin - {OriginCityIata}, " +
                       $"Destination - {DestinationCityIata}, " +
                       $"Departure at - {DepartureAt}, " +
                       $"Price - {Value}";
        }
}
