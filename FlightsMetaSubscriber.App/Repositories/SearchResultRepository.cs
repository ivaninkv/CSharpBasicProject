using Dapper;
using FlightsMetaSubscriber.App.Models;
using Npgsql;

namespace FlightsMetaSubscriber.App.Repositories;

public static class SearchResultRepository
{
    public static void SaveAll(this List<SearchResult> searchResults)
    {
        foreach (var searchResult in searchResults)
        {
            Save(searchResult);
        }
    }

    public static int Save(this SearchResult searchResult)
    {
        using var conn = new NpgsqlConnection(Config.ConnectionString);
        const string query =
            "insert into search_result (subscription_id, search_dt, origin, destination, departure_at, price, ticket_link, dt_offset, number_of_changes) " +
            "values (@subscription_id, @search_dt, @origin, @destination, @departure_at, @price, @ticket_link, @dt_offset, @number_of_changes) returning id";

        var insertedId = conn.ExecuteScalar<int>(query, new
        {
            subscription_id = searchResult.SubscriptionId,
            search_dt = searchResult.SearchDate,
            origin = searchResult.OriginCityIata,
            destination = searchResult.DestinationCityIata,
            departure_at = searchResult.DepartureAt.UtcDateTime,
            price = searchResult.Value,
            ticket_link = searchResult.TicketLink,
            dt_offset = searchResult.Offset,
            number_of_changes = searchResult.NumberOfChanges
        });

        return insertedId;
    }
}
