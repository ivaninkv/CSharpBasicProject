namespace FlightsMetaSubscriber.App.Models;

public class Subscription
{
    public Subscription()
    {
    }

    public Subscription(long userId)
    {
        UserId = userId;
    }

    public Subscription(
        int id,
        long userId,
        List<IataObject> origin,
        List<IataObject> destination,
        DateTime departureMinDate,
        DateTime departureMaxDate,
        DateTime? returnMinDate,
        DateTime? returnMaxDate,
        bool onlyDirect,
        bool baggage,
        bool active)
    {
        Id = id;
        UserId = userId;
        Origin = origin;
        Destination = destination;
        DepartureMinDate = departureMinDate;
        DepartureMaxDate = departureMaxDate;
        ReturnMinDate = returnMinDate;
        ReturnMaxDate = returnMaxDate;
        OnlyDirect = onlyDirect;
        Baggage = baggage;
        Active = active;
    }

    public int Id { get; set; }
    public long UserId { get; set; }
    public List<IataObject> Origin { get; set; } = new();
    public List<IataObject> Destination { get; set; } = new();
    public DateTime DepartureMinDate { get; set; }
    public DateTime DepartureMaxDate { get; set; }
    public DateTime? ReturnMinDate { get; set; }
    public DateTime? ReturnMaxDate { get; set; }
    public bool OnlyDirect { get; set; }
    public bool Baggage { get; set; } = false;
    public bool Active { get; set; } = true;

    public override string ToString()
    {
        return $"ID = {Id}\n" +
               $"Отправление - {string.Join(", ", Origin)}\n" +
               $"Прибытие - {string.Join(", ", Destination)}\n" +
               $"Вылет с - {DepartureMinDate:dd.MM.yyyy}\n" +
               $"Вылет по - {DepartureMaxDate:dd.MM.yyyy}\n" +
               $"Обратный вылет с {ReturnMinDate:dd.MM.yyyy}\n" +
               $"Обратный вылет по {ReturnMaxDate:dd.MM.yyyy}\n" +
               $"Только прямые рейсы - {OnlyDirect}\n" +
               $"Багаж включен - {Baggage}\n";
    }
}