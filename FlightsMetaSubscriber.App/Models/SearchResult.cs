namespace FlightsMetaSubscriber.App.Models;

public class SearchResult
{
    private const string SearchUrl = "https://www.aviasales.ru/search";

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

    public SearchResult()
    {
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
        return $"Вылет из - {OriginCityIata}, " +
               $"Прибытие в - {DestinationCityIata}, " +
               $"Вылет - {DepartureAt:dd.MM.yyyy}, " +
               $"Цена - {Value}\n" +
               $"[Купить билет]({GetFullUrl()})";
    }

    private string GetFullUrl()
    {
        return SearchUrl + TicketLink;
    }
}
