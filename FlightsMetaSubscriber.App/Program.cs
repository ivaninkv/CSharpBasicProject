using FlightsMetaSubscriber.App.AviasalesAPI;

public class Program
{
    public static async Task Main(string[] args)
    {
        var ac = new Autocomplete();
        var iataCodeByName = await ac.GetIataCodeByName("Москва");

        if (iataCodeByName != null)
            foreach (var item in iataCodeByName)
                Console.WriteLine($"{item.Code} - {item.Name}");
    }
}
