using System.Text.Json.Serialization;

namespace FlightsMetaSubscriber.App.AviasalesAPI;

public class AutocompleteResult
{
    [JsonPropertyName("type")] public string? Type { get; set; }

    [JsonPropertyName("code")] public string? Code { get; set; }

    [JsonPropertyName("name")] public string? Name { get; set; }

    [JsonPropertyName("country_name")] public string? CountryName { get; set; }
}
