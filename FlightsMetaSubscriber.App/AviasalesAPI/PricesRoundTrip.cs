using System.Globalization;
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
                double.Parse(node.Value["value"].ToString(), CultureInfo.InvariantCulture),
                node.Value["ticket_link"].ToString(),
                int.Parse(node.Value["number_of_changes"].ToString()),
                bool.Parse(node.Value["with_baggage"].ToString())
            )
        ).ToList();
    }

    public List<string> BuildQuery(Subscription subscription)
    {
        var queryList = new List<string>();
        
       
        foreach (var origin in subscription.Origin)
        {
            foreach (var destination in subscription.Destination)
            {
                var sb = new StringBuilder();
                sb.Append('{');
                sb.Append(string.Format(Config.PricesRoundTripQueryTemplate,
                    origin.Code + "_" + destination.Code,
                    origin.Code,
                    destination.Code,
                    subscription.DepartureMinDate.ToString(DateFormat),
                    subscription.DepartureMaxDate.ToString(DateFormat),
                    ((DateTime)subscription.ReturnMinDate).ToString(DateFormat),
                    ((DateTime)subscription.ReturnMaxDate).ToString(DateFormat),
                    subscription.OnlyDirect.ToString().ToLower(),
                    subscription.Baggage.ToString().ToLower()));
                sb.Append('}');
                var str = WhiteSpaces().Replace(sb.ToString(), " ");
                var result = $"{{ \"operationName\": null, \"variables\": {{}}, \"query\": \"{str}\" }}";

                _logger.LogDebug("Price_round_trip graphQL request {@PriceOneWayRequest}", result);

                queryList.Add(result);
            }
        }
        
        return queryList;
    }

    [GeneratedRegex("\\s+")]
    private static partial Regex WhiteSpaces();
}
