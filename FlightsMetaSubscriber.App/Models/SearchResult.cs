namespace FlightsMetaSubscriber.App.Models;

public class SearchResult
{
    public SearchResult(int subscriptionId, string originCityIata, string destinationCityIata,
        DateTimeOffset departureAt, double value, string ticketLink)
    {
        SubscriptionId = subscriptionId;
        OriginCityIata = originCityIata;
        DestinationCityIata = destinationCityIata;
        DepartureAt = departureAt;
        Value = value;
        TicketLink = ticketLink;
        SearchDate = DateTime.UtcNow;
        Offset = departureAt.Offset.ToString();
    }

    public int SubscriptionId { get; set; }
    public string OriginCityIata { get; set; }
    public string DestinationCityIata { get; set; }
    public DateTimeOffset DepartureAt { get; set; }
    public double Value { get; set; }
    public string TicketLink { get; set; }
    public DateTime SearchDate { get; set; }
    public string Offset { get; set; }

    public override string ToString()
    {
        return $"Origin - {OriginCityIata}, " +
               $"Destination - {DestinationCityIata}, " +
               $"Departure at - {DepartureAt}, " +
               $"Price - {Value}";
    }
}
