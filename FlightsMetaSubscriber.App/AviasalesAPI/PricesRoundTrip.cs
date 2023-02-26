using System.Text;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using FlightsMetaSubscriber.App.Models;
using Json.Path;
using RestSharp;

namespace FlightsMetaSubscriber.App.AviasalesAPI;

public partial class PricesRoundTrip : IGetPrices
{
    private const string DateFormat = "yyyy-MM-dd";
    private readonly ILogger<PricesRoundTrip> _logger;

    public PricesRoundTrip(ILogger<PricesRoundTrip> logger)
    {
        _logger = logger;
    }

    public List<SearchResult> DeserializeNodeList(NodeList nodeList, Subscription subscription)
    {
        return nodeList.Select(node => new SearchResult(
                subscription.Id,
                node.Value["origin_city_iata"].ToString(),
                node.Value["destination_city_iata"].ToString(),
                DateTimeOffset.ParseExact(node.Value["departure_at"].ToString(), "yyyy-MM-ddTHH:mm:sszzz", null),
                DateTimeOffset.ParseExact(node.Value["return_at"].ToString(), "yyyy-MM-ddTHH:mm:sszzz", null),
                double.Parse(node.Value["value"].ToString()),
                node.Value["ticket_link"].ToString(),
                int.Parse(node.Value["number_of_changes"].ToString())
            )
        ).ToList();
    }

    public string BuildQuery(Subscription subscription)
    {
        var sb = new StringBuilder();
        sb.Append('{');
        foreach (var origin in subscription.Origin)
        {
            foreach (var destination in subscription.Destination)
            {
                sb.Append(string.Format(Config.PricesRoundTripQueryTemplate,
                    origin.Code + "_" + destination.Code,
                    origin.Code,
                    destination.Code,
                    subscription.DepartureMinDate.ToString(DateFormat),
                    subscription.DepartureMaxDate.ToString(DateFormat),
                    ((DateTime)subscription.ReturnMinDate).ToString(DateFormat),
                    ((DateTime)subscription.ReturnMaxDate).ToString(DateFormat),
                    subscription.OnlyDirect.ToString().ToLower()));
            }
        }

        sb.Append('}');

        var str = WhiteSpaces().Replace(sb.ToString(), " ");
        var result = $"{{ \"operationName\": null, \"variables\": {{}}, \"query\": \"{str}\" }}";

        _logger.LogDebug("Price_round_trip graphQL request {@PriceOneWayRequest}", result);

        return result;
    }

    [GeneratedRegex("\\s+")]
    private static partial Regex WhiteSpaces();
}
