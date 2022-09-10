using System.Text.Json;

namespace FlightsMetaSubscriber.App.AviasalesAPI;

public class Autocomplete
{
    private const string AutocompleteUrl =
        "https://autocomplete.travelpayouts.com/places2?locale=ru&types[]=city&types[]=airport&term=";

    private static readonly HttpClient Client = new();

    public async Task<IEnumerable<AutocompleteResult>?> GetIataCodeByName(string searchRequest)
    {
        var streamTask = Client.GetStreamAsync(AutocompleteUrl + searchRequest);
        var results = await JsonSerializer.DeserializeAsync<List<AutocompleteResult>>(await streamTask);
        return results?.Take(3);
    }
}
