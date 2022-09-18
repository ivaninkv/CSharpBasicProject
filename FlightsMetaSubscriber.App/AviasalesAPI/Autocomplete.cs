using System.Text.Json;

namespace FlightsMetaSubscriber.App.AviasalesAPI;

public class Autocomplete
{
    private const string AutocompleteUrl =
        "https://autocomplete.travelpayouts.com/places2?locale=ru&types[]=city&types[]=airport&term=";

    private static readonly HttpClient Client = new();
    private readonly ILogger<Autocomplete> _logger;

    public Autocomplete(ILogger<Autocomplete> logger)
    {
        _logger = logger;
    }

    public async Task<IEnumerable<AutocompleteResult>> GetIataCodeByName(string searchRequest)
    {
        try
        {
            _logger.LogInformation("Trying to get data from Aviasales API...");
            var streamTask = Client.GetStreamAsync(AutocompleteUrl + searchRequest);
            var results = await JsonSerializer.DeserializeAsync<List<AutocompleteResult>>(await streamTask);
            return results?.Take(3) ?? Enumerable.Empty<AutocompleteResult>();
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
        }

        return Enumerable.Empty<AutocompleteResult>();
    }
}
