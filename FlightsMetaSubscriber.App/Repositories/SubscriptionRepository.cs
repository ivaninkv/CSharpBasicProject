using Dapper;
using FlightsMetaSubscriber.App.Models;
using Npgsql;

namespace FlightsMetaSubscriber.App.Repositories;

public class SubscriptionRepository
{
    public static void Save(Subscription subscription)
    {
        using var conn = new NpgsqlConnection(Config.ConnectionString);
        const string subscriptionQuery =
            $"insert into subscription(user_id, departure_min_date, departure_max_date) " +
            $"values (@user_id, @departure_min_date, @departure_max_date) returning id";
        var insertedId = conn.ExecuteScalar<int>(subscriptionQuery, new
        {
            user_id = subscription.UserId,
            departure_min_date = subscription.DepartureMinDate,
            departure_max_date = subscription.DepartureMaxDate
        });


        const string originQuery = $"insert into origin(subscribe_id, user_id, iata_code) " +
                                   $"values (@subscribe_id, @user_id, @iata_code)";
        foreach (var iataObject in subscription.Origin)
        {
            conn.Execute(originQuery, new
            {
                subscribe_id = insertedId,
                user_id = subscription.UserId,
                iata_code = iataObject.Code
            });
        }

        const string destinationQuery = $"insert into destination(subscribe_id, user_id, iata_code) " +
                                        $"values (@subscribe_id, @user_id, @iata_code)";
        foreach (var iataObject in subscription.Destination)
        {
            conn.Execute(destinationQuery, new
            {
                subscribe_id = insertedId,
                user_id = subscription.UserId,
                iata_code = iataObject.Code
            });
        }
    }
}
