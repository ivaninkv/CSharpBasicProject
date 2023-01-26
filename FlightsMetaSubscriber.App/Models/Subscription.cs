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
        return $"Origin - {string.Join(", ", Origin)}; " +
               $"Destination - {string.Join(", ", Destination)}; " +
               $"DepartureMinDate - {DepartureMinDate}; " +
               $"DepartureMaxDate - {DepartureMaxDate}.";
    }
}
