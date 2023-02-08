namespace FlightsMetaSubscriber.App.Models;

public class Subscription
{
    public int Id { get; set; }
    public long UserId { get; set; }
    public List<IataObject> Origin { get; set; }
    public List<IataObject> Destination { get; set; }
    public DateTime DepartureMinDate { get; set; }
    public DateTime DepartureMaxDate { get; set; }
    public bool OnlyDirect { get; set; }
    public bool Active { get; set; }

    public override string ToString()
    {
        return $"Отправление - {string.Join(", ", Origin)}\n" +
               $"Прибытие - {string.Join(", ", Destination)}\n" +
               $"Вылет с - {DepartureMinDate:dd.MM.yyyy}\n" +
               $"Вылет по - {DepartureMaxDate:dd.MM.yyyy}\n" +
               $"Только прямые рейсы - {OnlyDirect}";
    }
}
