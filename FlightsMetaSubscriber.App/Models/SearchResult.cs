namespace FlightsMetaSubscriber.App.Models;

public class SearchResult
{
    private static readonly string SearchUrl = "https://www.aviasales.ru/search";

    public SearchResult(
        int subscriptionId,
        string originCityIata,
        string destinationCityIata,
        DateTimeOffset departureAt,
        DateTimeOffset? returnAt,
        double value,
        string ticketLink,
        int numberOfChanges)
    {
        SubscriptionId = subscriptionId;
        OriginCityIata = originCityIata;
        DestinationCityIata = destinationCityIata;
        DepartureAt = departureAt;
        ReturnAt = returnAt;
        Value = value;
        TicketLink = ticketLink;
        SearchDate = DateTime.UtcNow;
        Offset = departureAt.Offset.ToString();
        NumberOfChanges = numberOfChanges;
    }

    public SearchResult()
    {
    }

    public int SubscriptionId { get; init; }
    public string OriginCityIata { get; set; }
    public string DestinationCityIata { get; set; }
    public DateTimeOffset DepartureAt { get; set; }
    public DateTimeOffset? ReturnAt { get; set; }
    public double Value { get; set; }
    public string TicketLink { get; set; }
    public DateTime SearchDate { get; set; }
    public string Offset { get; set; }
    public int NumberOfChanges { get; set; }

    public override string ToString()
    {
        return $"Вылет из - {OriginCityIata}\n" +
               $"Прибытие в - {DestinationCityIata}\n" +
               $"Вылет - {DepartureAt:dd.MM.yyyy}\n" +
               $"Возвращение - {ReturnAt:dd.MM.yyyy}\n" +
               $"Количество пересадок - {NumberOfChanges}\n" +
               $"Цена - {Value}\n" +
               $"[Купить билет]({GetFullUrl()})";
    }

    private string GetFullUrl()
    {
        return SearchUrl + TicketLink;
    }
}