using System.Text;
using FlightsMetaSubscriber.App.Models;
using RestSharp;

namespace FlightsMetaSubscriber.App.AviasalesAPI;

public class PricesOneWay
{
    private const string GraphQlUrl = "http://api.travelpayouts.com/graphql/v1/query";

    public async Task GetMinPrices(Subscription subscription)
    {
        var client = new RestClient(GraphQlUrl);
        var request = new RestRequest("", Method.Post)
            .AddHeader("Content-Type", "application/json")
            .AddHeader("X-Access-token", Config.AviaSalesApiToken)
            .AddParameter("application/json", BuildQuery(subscription), ParameterType.RequestBody);
        var result = await client.PostAsync(request);
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

        return $"{{ \"operationName\": null, \"variables\": {{}}, \"query\": \"{sb}\" }}";
    }
}
