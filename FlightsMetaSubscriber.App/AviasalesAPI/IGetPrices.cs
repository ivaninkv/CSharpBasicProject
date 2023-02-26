using FlightsMetaSubscriber.App.Models;
using Json.Path;

namespace FlightsMetaSubscriber.App.AviasalesAPI;

public interface IGetPrices
{
    public string BuildQuery(Subscription subscription);

    public List<SearchResult> DeserializeNodeList(NodeList nodeList, Subscription subscription);
}