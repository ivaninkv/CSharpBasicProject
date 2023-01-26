namespace FlightsMetaSubscriber.App.Models;

public class Subscription
{
    public int Id { get; set; }
    public List<IataObject> Origin { get; set; }
    public List<IataObject> Destination { get; set; }
    public DateOnly DepartureMinDate { get; set; }
    public DateOnly DepartureMaxDate { get; set; }

    public override string ToString()
    {
        return $"Отправление - {string.Join(", ", Origin)}; " +
               $"Прибытие - {string.Join(", ", Destination)}; " +
               $"Вылет с - {DepartureMinDate}; " +
               $"Вылет по - {DepartureMaxDate}.";
    }
}
