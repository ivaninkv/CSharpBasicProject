using System.Text;
using System.Text.Json.Nodes;
using FlightsMetaSubscriber.App.Models;
using Json.Path;
using RestSharp;

namespace FlightsMetaSubscriber.App.AviasalesAPI;

public class PricesOneWay
{
    private const string GraphQlUrl = "http://api.travelpayouts.com/graphql/v1/query";
    private readonly ILogger<PricesOneWay> _logger;

    public PricesOneWay(ILogger<PricesOneWay> logger)
    {
        _logger = logger;
    }

    public async Task<List<SearchResult>> FindPricesForSubscription(Subscription subscription)
    {
        var client = new RestClient(GraphQlUrl);
        var request = new RestRequest("", Method.Post)
            .AddHeader("Content-Type", "application/json")
            .AddHeader("X-Access-token", Config.AviaSalesApiToken)
            .AddParameter("application/json", BuildQuery(subscription), ParameterType.RequestBody);
        var result = await client.PostAsync(request);

        var searchResults = new List<SearchResult>();
        try
        {
            var data = JsonNode.Parse(result.Content);
            var nodeList = JsonPath.Parse("$.*.*.*").Evaluate(data).Matches;
            // var searchResults = nodeList.ToJsonDocument().Deserialize<List<SearchResult>>();
            searchResults = DeserializeNodeList(nodeList, subscription.Id);
        }
        catch (Exception e)
        {
            _logger.LogInformation("Error parsing JSON from AviaSales, message: {@ErrorMessage}", e.Message);
        }

        return searchResults;
    }

    private List<SearchResult> DeserializeNodeList(NodeList nodeList, int subscriptionId)
    {
        return nodeList.Select(node => new SearchResult(
            subscriptionId,
            node.Value["origin_city_iata"].ToString(),
            node.Value["destination_city_iata"].ToString(),
            DateTimeOffset.ParseExact(node.Value["departure_at"].ToString(), "yyyy-MM-ddTHH:mm:sszzz", null),
            double.Parse(node.Value["value"].ToString()),
            node.Value["ticket_link"].ToString())).ToList();
    }

    private string BuildQuery(Subscription subscription)
    {
        var sb = new StringBuilder();
        sb.Append('{');
        foreach (var origin in subscription.Origin)
        {
            foreach (var destination in subscription.Destination)
            {
                sb.Append(string.Format(Config.QueryTemplate,
                    origin.Code + "_" + destination.Code,
                    origin.Code,
                    destination.Code,
                    subscription.DepartureMinDate.ToString("yyyy-MM-dd"),
                    subscription.DepartureMaxDate.ToString("yyyy-MM-dd")));
            }
        }

        sb.Append('}');
        sb.Replace("\r", string.Empty)
            .Replace("\n", string.Empty)
            .Replace("\t", " ");

        var result = $"{{ \"operationName\": null, \"variables\": {{}}, \"query\": \"{sb}\" }}";

        _logger.LogDebug("Price_one_way graphQL request {@PriceOneWayRequest}", result);

        return result;
    }
}
