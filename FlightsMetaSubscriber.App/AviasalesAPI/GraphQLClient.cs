using System.Text.Json.Nodes;
using FlightsMetaSubscriber.App.Models;
using Json.Path;
using RestSharp;

namespace FlightsMetaSubscriber.App.AviasalesAPI;

public class GraphQLClient
{
    private const string GraphQlUrl = "http://api.travelpayouts.com/graphql/v1/query";
    private readonly ILogger<GraphQLClient> _logger;
    private readonly PricesOneWay _pricesOneWay;
    private readonly PricesRoundTrip _pricesRoundTrip;

    public GraphQLClient(
        ILogger<GraphQLClient> logger, 
        PricesOneWay pricesOneWay, 
        PricesRoundTrip pricesRoundTrip)
    {
        _logger = logger;
        _pricesOneWay = pricesOneWay;
        _pricesRoundTrip = pricesRoundTrip;
    }

    public async Task<List<SearchResult>> FindPricesForSubscription(Subscription subscription)
    {
        var client = new RestClient(GraphQlUrl);
        var request = new RestRequest("", Method.Post)
            .AddHeader("Content-Type", "application/json")
            .AddHeader("X-Access-token", Config.AviaSalesApiToken)
            .AddParameter("application/json", BuildQuery(subscription), ParameterType.RequestBody);

        try
        {
            var result = await client.PostAsync(request);
            var data = JsonNode.Parse(result.Content);
            var nodeList = JsonPath.Parse("$.*.*.*").Evaluate(data).Matches;
            return DeserializeNodeList(nodeList, subscription);
        }
        catch (Exception e)
        {
            _logger.LogWarning("Error from AviaSales, message: {@ErrorMessage}", e.Message);
            return new List<SearchResult>();
        }
    }

    private List<SearchResult> DeserializeNodeList(NodeList nodeList, Subscription subscription)
    {
        if (subscription.ReturnMaxDate is null || subscription.ReturnMinDate is null)
        {
            return _pricesOneWay.DeserializeNodeList(nodeList, subscription);
        }
        else
        {
            return _pricesRoundTrip.DeserializeNodeList(nodeList, subscription);
        }
    }

    private string BuildQuery(Subscription subscription)
    {
        if (subscription.ReturnMaxDate is null || subscription.ReturnMinDate is null)
        {
            return _pricesOneWay.BuildQuery(subscription);
        }
        else
        {
            return _pricesRoundTrip.BuildQuery(subscription);
        }
    }
}