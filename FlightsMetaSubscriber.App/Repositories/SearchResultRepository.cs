using Dapper;
using FlightsMetaSubscriber.App.Models;
using Npgsql;

namespace FlightsMetaSubscriber.App.Repositories;

public class SearchResultRepository
{
    public static void SaveAll(List<SearchResult> searchResults)
    {
        foreach (var searchResult in searchResults)
        {
            Save(searchResult);
        }
    }

    public static int Save(SearchResult searchResult)
    {
        using var conn = new NpgsqlConnection(Config.ConnectionString);
        const string query =
            $"insert into search_result (subscription_id, search_dt, origin, destination, departure_at, price, ticket_link, dt_offset) " +
            $"values (@subscription_id, @search_dt, @origin, @destination, @departure_at, @price, @ticket_link, @dt_offset) returning id";
        var insertedId = conn.ExecuteScalar<int>(query, new
        {
            subscription_id = searchResult.SubscriptionId,
            search_dt = searchResult.SearchDate,
            origin = searchResult.OriginCityIata,
            destination = searchResult.DestinationCityIata,
            departure_at = searchResult.DepartureAt.UtcDateTime,
            price = searchResult.Value,
            ticket_link = searchResult.TicketLink,
            dt_offset = searchResult.Offset
        });

        return insertedId;
    }
}
